using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpinionsAnalytics.Application.DTO;
using OpinionsAnalytics.Application.Repositories;
using OpinionsAnalytics.Application.Result;
using OpinionsAnalytics.Domain.Entities.Dwh.Dimensions;
using OpinionsAnalytics.Domain.Entities.Dwh.Facts;
using OpinionsAnalytics.Persistence.Context;
using OpinionsAnalytics.Domain.Entities.Db;

namespace OpinionsAnalytics.Persistence.Repositories.Dwh
{
    public class DwhRepository : IDwhRepository
    {
        private readonly DwhRepositoryContext _dwhContext;
        private readonly ResenasContext _resenasContext;
        private readonly ILogger<DwhRepository> _logger;
        private const int FactBatchSize = 1000;

        public DwhRepository(DwhRepositoryContext dwhRepositoryContext,
                             ResenasContext resenasContext,
                             ILogger<DwhRepository> logger)
        {
            _dwhContext = dwhRepositoryContext ?? throw new ArgumentNullException(nameof(dwhRepositoryContext));
            _resenasContext = resenasContext ?? throw new ArgumentNullException(nameof(resenasContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult> LoadDimensionsAsync(IEnumerable<DimClienteDto>? clientes = null,
                                                            IEnumerable<DimFuenteDto>? fuentes = null,
                                                            IEnumerable<DimClasificacionDto>? clasificaciones = null,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                if (clientes != null)
                    await UpsertDimClientesAsync(clientes, cancellationToken);

                if (fuentes != null)
                    await UpsertDimFuentesAsync(fuentes, cancellationToken);

                if (clasificaciones != null)
                    await UpsertDimClasificacionesAsync(clasificaciones, cancellationToken);

                return new ServiceResult { IsSucess = true, Message = "Dimensions loaded" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dimensions");
                return new ServiceResult { IsSucess = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Carga facts a partir de Resenas. Antes de insertar facts asegura que las dimensiones clave
        /// (Fecha, Producto, Cliente, Fuente, Clasificacion) existan en el DWH.
        /// </summary>
        public async Task<ServiceResult> LoadFactsFromResenasAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // 1) extraer reseñas fuente
                var resenas = await _resenasContext.Resenas.AsNoTracking().ToListAsync(cancellationToken);

                if (!resenas.Any())
                    return new ServiceResult { IsSucess = true, Message = "No resenas to process" };

                // 2) construir y asegurar dimensiones en orden:
                //    DimFecha -> DimProducto -> DimCliente -> DimFuente/DimClasificacion (si aplica)
                var dimFechas = BuildDimFechaFromResenas(resenas);
                await UpsertDimFechasAsync(dimFechas, cancellationToken);

                var dimProductos = BuildDimProductosFromResenas(resenas);
                await UpsertDimProductosAsync(dimProductos, cancellationToken);

                var dimClientes = BuildDimClientesFromResenas(resenas);
                await UpsertDimClientesAsync(dimClientes, cancellationToken);

                // NUEVO: Asegurar que existe DimFuente y obtener su key
                var defaultFuenteKey = await EnsureDefaultFuenteAsync(cancellationToken);

                // 3) construir mapas de resolución (surrogate keys)
                var fechaMap = await BuildFechaKeyMapAsync(dimFechas.Select(f => f.FechaKey), cancellationToken);
                var productoMap = await BuildProductoKeyMapAsync(dimProductos.Select(p => p.IdProducto), cancellationToken);
                var clienteMap = await BuildClienteKeyMapAsync(dimClientes.Select(d => d.IdCliente).Distinct(), cancellationToken);

                // 4) mapear resenas -> facts usando los maps
                var factDtos = BuildFactOpinionDtosFromResenas(resenas, clienteMap, productoMap, fechaMap, defaultFuenteKey);

                // 5) insertar facts por batches
                await LoadFactsAsync(factDtos, cancellationToken);

                return new ServiceResult { IsSucess = true, Message = $"Loaded {factDtos.Count} facts" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading facts from Resenas");
                return new ServiceResult { IsSucess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult> LoadFactsAsync(IEnumerable<FactOpinionDto> facts, CancellationToken cancellationToken = default)
        {
            try
            {
                var list = facts.ToList();
                if (!list.Any()) return new ServiceResult { IsSucess = true, Message = "No facts to load" };

                var dbSet = _dwhContext.Set<FactOpinion>();

                for (int i = 0; i < list.Count; i += FactBatchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var batch = list.Skip(i).Take(FactBatchSize)
                        .Select(dto => new FactOpinion
                        {
                            // OpinionKey left for DB identity
                            ClienteKey = dto.ClienteKey,
                            ProductoKey = dto.ProductoKey,
                            FechaKey = dto.FechaKey,
                            FuenteKey = dto.FuenteKey,
                            ClasificacionKey = dto.ClasificacionKey,
                            IdOpinionOriginal = dto.IdOpinionOriginal,
                            Rating = dto.Rating,
                            PuntajeSatisfaccion = dto.PuntajeSatisfaccion,
                            Comentario = dto.Comentario,
                            LongitudComentario = dto.LongitudComentario,
                            TieneComentario = dto.TieneComentario,
                            FechaCargaDw = dto.FechaCargaDw ?? DateTime.UtcNow,
                            FuenteSistema = dto.FuenteSistema
                        })
                        .ToList();

                    await dbSet.AddRangeAsync(batch, cancellationToken);
                    await _dwhContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Inserted {count} facts", batch.Count);
                }

                return new ServiceResult { IsSucess = true, Message = "Facts loaded" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading facts");
                return new ServiceResult { IsSucess = false, Message = ex.Message };
            }
        }

        #region Builders and Upserts for Fecha/Producto/Cliente

        private static List<DimClienteDto> BuildDimClientesFromResenas(IEnumerable<Resenas> resenas)
        {
            return resenas
                .Select(r => new DimClienteDto(r.IdCliente, null, null, null, true))
                .GroupBy(d => d.IdCliente)
                .Select(g => g.First())
                .ToList();
        }

        private static List<DimFecha> BuildDimFechaFromResenas(IEnumerable<Resenas> resenas)
        {
            var dates = resenas.Select(r => r.Fecha).Distinct();
            var list = new List<DimFecha>();
            foreach (var d in dates)
            {
                var fechaKey = BuildFechaKey(d);
                var fecha = new DimFecha
                {
                    FechaKey = fechaKey,
                    Fecha = d,
                    Anio = (short)d.Year,
                    Mes = (byte)d.Month,
                    Trimestre = (byte)((d.Month - 1) / 3 + 1),
                    NombreMes = d.ToString("MMMM"),
                    Semana = (byte)System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        d.ToDateTime(TimeOnly.MinValue), System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                    DiaMes = (byte)d.Day,
                    DiaSemana = (byte)((int)d.DayOfWeek == 0 ? 7 : (int)d.DayOfWeek),
                    NombreDiaSemana = d.ToString("dddd"),
                    EsFinDeSemana = (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday),
                    EsFeriado = false
                };
                list.Add(fecha);
            }
            return list;
        }

        private static List<DimProducto> BuildDimProductosFromResenas(IEnumerable<Resenas> resenas)
        {
            return resenas.Select(r => r.IdProducto)
                          .Distinct()
                          .Select(id => new DimProducto
                          {
                              IdProducto = id,
                              NombreProducto = $"Producto {id}",
                              RegistroActivo = true
                          })
                          .ToList();
        }

        private async Task UpsertDimFechasAsync(IEnumerable<DimFecha> fechas, CancellationToken ct)
        {
            var incoming = fechas.ToList();
            if (!incoming.Any()) return;

            var dbSet = _dwhContext.Set<DimFecha>();
            var keys = incoming.Select(x => x.FechaKey).Distinct().ToList();
            var existing = await dbSet.Where(d => keys.Contains(d.FechaKey)).ToDictionaryAsync(d => d.FechaKey, ct);

            foreach (var src in incoming)
            {
                if (!existing.ContainsKey(src.FechaKey))
                {
                    await dbSet.AddAsync(src, ct);
                }
            }

            await _dwhContext.SaveChangesAsync(ct);
        }

        private async Task UpsertDimProductosAsync(IEnumerable<DimProducto> productos, CancellationToken ct)
        {
            var incoming = productos.ToList();
            if (!incoming.Any()) return;

            var dbSet = _dwhContext.Set<DimProducto>();
            var ids = incoming.Select(x => x.IdProducto).Distinct().ToList();
            var existing = await dbSet.Where(d => ids.Contains(d.IdProducto)).ToDictionaryAsync(d => d.IdProducto, ct);

            foreach (var src in incoming)
            {
                if (existing.TryGetValue(src.IdProducto, out var ex))
                {
                    ex.NombreProducto = src.NombreProducto ?? ex.NombreProducto;
                    ex.RegistroActivo = src.RegistroActivo ?? ex.RegistroActivo;
                    ex.FechaActualizacion = DateTime.UtcNow;
                    _dwhContext.Update(ex);
                }
                else
                {
                    await dbSet.AddAsync(src, ct);
                }
            }

            await _dwhContext.SaveChangesAsync(ct);
        }

        private async Task<Dictionary<int, int>> BuildFechaKeyMapAsync(IEnumerable<int> fechaKeys, CancellationToken ct)
        {
            var keys = fechaKeys.Distinct().ToList();
            var dbSet = _dwhContext.Set<DimFecha>();
            var items = await dbSet.Where(d => keys.Contains(d.FechaKey)).AsNoTracking().ToListAsync(ct);
            return items.ToDictionary(i => i.FechaKey, i => i.FechaKey);
        }

        private async Task<Dictionary<int, int>> BuildProductoKeyMapAsync(IEnumerable<int> idProductos, CancellationToken ct)
        {
            var ids = idProductos.Distinct().ToList();
            var dbSet = _dwhContext.Set<DimProducto>();
            var items = await dbSet.Where(d => ids.Contains(d.IdProducto)).AsNoTracking().ToListAsync(ct);
            return items.ToDictionary(i => i.IdProducto, i => i.ProductoKey);
        }

        private async Task<Dictionary<int, int>> BuildClienteKeyMapAsync(IEnumerable<int> idClientes, CancellationToken ct)
        {
            var keys = idClientes.Distinct().ToList();
            var dbSet = _dwhContext.Set<DimCliente>();
            var items = await dbSet.Where(d => keys.Contains(d.IdCliente)).AsNoTracking().ToListAsync(ct);
            return items.ToDictionary(i => i.IdCliente, i => i.ClienteKey);
        }

        #endregion

        #region DimFuente Helper

        /// <summary>
        /// Asegura que existe una fuente por defecto en DimFuente y retorna su FuenteKey.
        /// </summary>
        private async Task<int> EnsureDefaultFuenteAsync(CancellationToken ct)
        {
            const string defaultFuenteId = "WEB_REVIEWS";

            var dbSet = _dwhContext.Set<DimFuente>();
            var fuente = await dbSet.FirstOrDefaultAsync(f => f.IdFuente == defaultFuenteId, ct);

            if (fuente == null)
            {
                fuente = new DimFuente
                {
                    IdFuente = defaultFuenteId,
                    TipoFuente = "Web",
                    Canal = "Online",
                    Descripcion = "Reseñas del sitio web",
                    FechaCarga = DateOnly.FromDateTime(DateTime.UtcNow),
                    RegistroActivo = true
                };

                await dbSet.AddAsync(fuente, ct);
                await _dwhContext.SaveChangesAsync(ct);

                _logger.LogInformation("Default DimFuente created with FuenteKey: {FuenteKey}", fuente.FuenteKey);
            }

            return fuente.FuenteKey;
        }

        #endregion

        #region Build FactOpinion DTOs

        private List<FactOpinionDto> BuildFactOpinionDtosFromResenas(IEnumerable<Resenas> resenas, Dictionary<int, int> clienteMap)
        {
            // kept for backward compatibility: if only clienteMap provided
            return BuildFactOpinionDtosFromResenas(resenas, clienteMap, new Dictionary<int, int>(), new Dictionary<int, int>(), 0);
        }

        private List<FactOpinionDto> BuildFactOpinionDtosFromResenas(
            IEnumerable<Resenas> resenas,
            Dictionary<int, int> clienteMap,
            Dictionary<int, int> productoMap,
            Dictionary<int, int> fechaMap,
            int fuenteKey)
        {
            var now = DateTime.UtcNow;
            var result = new List<FactOpinionDto>();

            foreach (var r in resenas)
            {
                var fechaKeyInt = BuildFechaKey(r.Fecha);
                fechaMap.TryGetValue(fechaKeyInt, out var fechaSurrogateKey);
                productoMap.TryGetValue(r.IdProducto, out var productoSurrogateKey);
                clienteMap.TryGetValue(r.IdCliente, out var clienteSurrogateKey);

                var comentario = string.IsNullOrWhiteSpace(r.Comentario) ? null : r.Comentario;
                var tieneComentario = !string.IsNullOrWhiteSpace(comentario);
                var longitud = tieneComentario ? comentario!.Length : 0;

                var dto = new FactOpinionDto(
                    OpinionKey: null,
                    ClienteKey: clienteSurrogateKey == 0 ? null : (int?)clienteSurrogateKey,
                    ProductoKey: productoSurrogateKey == 0 ? r.IdProducto : productoSurrogateKey,
                    FechaKey: fechaSurrogateKey == 0 ? fechaKeyInt : fechaSurrogateKey,
                    FuenteKey: fuenteKey,  // Usar el fuenteKey real obtenido de la BD
                    ClasificacionKey: null,
                    IdOpinionOriginal: r.IdReview.ToString(),
                    Rating: r.Rating,
                    PuntajeSatisfaccion: null,
                    Comentario: comentario,
                    LongitudComentario: longitud,
                    TieneComentario: tieneComentario,
                    FechaCargaDw: now,
                    FuenteSistema: "Resenas"
                );
                result.Add(dto);
            }

            return result;
        }

        #endregion

        #region Upserts for Cliente/Fuente/Clasificacion

        private async Task UpsertDimClientesAsync(IEnumerable<DimClienteDto> clientes, CancellationToken ct)
        {
            var incoming = clientes.ToList();
            if (!incoming.Any()) return;

            var dbSet = _dwhContext.Set<DimCliente>();
            var keys = incoming.Select(x => x.IdCliente).Distinct().ToList();

            var existing = await dbSet.Where(d => keys.Contains(d.IdCliente)).ToDictionaryAsync(d => d.IdCliente, ct);

            foreach (var src in incoming)
            {
                if (existing.TryGetValue(src.IdCliente, out var ex))
                {
                    ex.Nombre = src.Nombre ?? ex.Nombre;
                    ex.Email = src.Email ?? ex.Email;
                    ex.RegistroActivo = src.RegistroActivo ?? ex.RegistroActivo;
                    ex.FechaActualizacion = DateTime.UtcNow;
                    _dwhContext.Update(ex);
                }
                else
                {
                    var newEntity = new DimCliente
                    {
                        IdCliente = src.IdCliente,
                        Nombre = src.Nombre,
                        Email = src.Email,
                        FechaCreacion = src.FechaCreacion,
                        FechaActualizacion = DateTime.UtcNow,
                        RegistroActivo = src.RegistroActivo ?? true
                    };
                    await dbSet.AddAsync(newEntity, ct);
                }
            }

            await _dwhContext.SaveChangesAsync(ct);
        }

        private async Task UpsertDimFuentesAsync(IEnumerable<DimFuenteDto> fuentes, CancellationToken ct)
        {
            var incoming = fuentes.ToList();
            if (!incoming.Any()) return;

            var dbSet = _dwhContext.Set<DimFuente>();
            var keys = incoming.Select(x => x.IdFuente).Distinct().ToList();

            var existing = await dbSet.Where(d => keys.Contains(d.IdFuente)).ToDictionaryAsync(d => d.IdFuente!, ct);

            foreach (var src in incoming)
            {
                if (existing.TryGetValue(src.IdFuente, out var ex))
                {
                    ex.TipoFuente = src.TipoFuente ?? ex.TipoFuente;
                    ex.Canal = src.Canal ?? ex.Canal;
                    ex.Descripcion = src.Descripcion ?? ex.Descripcion;
                    ex.RegistroActivo = src.RegistroActivo ?? ex.RegistroActivo;
                    _dwhContext.Update(ex);
                }
                else
                {
                    var newEntity = new DimFuente
                    {
                        IdFuente = src.IdFuente,
                        TipoFuente = src.TipoFuente,
                        Canal = src.Canal,
                        Descripcion = src.Descripcion,
                        FechaCarga = DateOnly.FromDateTime(DateTime.UtcNow),
                        RegistroActivo = src.RegistroActivo ?? true
                    };
                    await dbSet.AddAsync(newEntity, ct);
                }
            }

            await _dwhContext.SaveChangesAsync(ct);
        }

        private async Task UpsertDimClasificacionesAsync(IEnumerable<DimClasificacionDto> clasificaciones, CancellationToken ct)
        {
            var incoming = clasificaciones.ToList();
            if (!incoming.Any()) return;

            var dbSet = _dwhContext.Set<DimClasificacion>();
            var keys = incoming.Select(x => x.Clasificacion).Distinct().ToList();

            var existing = await dbSet.Where(d => keys.Contains(d.Clasificacion)).ToDictionaryAsync(d => d.Clasificacion!, ct);

            foreach (var src in incoming)
            {
                if (existing.TryGetValue(src.Clasificacion, out var ex))
                {
                    ex.Descripcion = src.Descripcion ?? ex.Descripcion;
                    ex.ValorNumerico = src.ValorNumerico ?? ex.ValorNumerico;
                    _dwhContext.Update(ex);
                }
                else
                {
                    var newEntity = new DimClasificacion
                    {
                        Clasificacion = src.Clasificacion,
                        Descripcion = src.Descripcion,
                        ValorNumerico = src.ValorNumerico
                    };
                    await dbSet.AddAsync(newEntity, ct);
                }
            }

            await _dwhContext.SaveChangesAsync(ct);
        }

        #endregion

        #region Utility Methods

        private static int BuildFechaKey(DateOnly fecha) =>
            fecha.Year * 10000 + fecha.Month * 100 + fecha.Day;

        #endregion
    }
}