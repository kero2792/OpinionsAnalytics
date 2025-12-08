using System;
using System.Collections.Generic;

namespace OpinionsAnalytics.Application.DTO
{
    public record DimClienteDto(int IdCliente, string? Nombre, string? Email, DateOnly? FechaCreacion, bool? RegistroActivo);

    public record DimFuenteDto(string IdFuente, string? TipoFuente, string? Canal, string? Descripcion, bool? RegistroActivo);

    public record DimClasificacionDto(string Clasificacion, string? Descripcion, byte? ValorNumerico);

    public record FactOpinionDto(
        long? OpinionKey,
        int? ClienteKey,
        int ProductoKey,
        int FechaKey,
        int FuenteKey,
        int? ClasificacionKey,
        string IdOpinionOriginal,
        decimal? Rating,
        short? PuntajeSatisfaccion,
        string? Comentario,
        int? LongitudComentario,
        bool? TieneComentario,
        DateTime? FechaCargaDw,
        string? FuenteSistema);
}