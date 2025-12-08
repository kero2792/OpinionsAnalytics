using Microsoft.EntityFrameworkCore;
using OpinionsAnalytics.Domain.Entities.Dwh.Dimensions;
using OpinionsAnalytics.Domain.Entities.Dwh.Facts;

namespace OpinionsAnalytics.Persistence.Context
{
    public class DwhRepositoryContext : DbContext
    {
        public DwhRepositoryContext(DbContextOptions<DwhRepositoryContext> options) : base(options)
        {
        }

        public DbSet<DimCliente> DimClientes { get; set; } = null!;
        public DbSet<DimFuente> DimFuentes { get; set; } = null!;
        public DbSet<DimClasificacion> DimClasificaciones { get; set; } = null!;
        public DbSet<DimFecha> DimFechas { get; set; } = null!;
        public DbSet<DimProducto> DimProductos { get; set; } = null!;
        public DbSet<FactOpinion> FactOpinions { get; set; } = null!;
        public DbSet<FactOpinionDiarium> FactOpinionDiariums { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // DIMENSIONES
            modelBuilder.Entity<DimCliente>(entity =>
            {
                entity.ToTable("DimCliente", "dbo");
                entity.HasKey(d => d.ClienteKey);
            });

            modelBuilder.Entity<DimFuente>(entity =>
            {
                entity.ToTable("DimFuente", "dbo");
                entity.HasKey(d => d.FuenteKey);
            });

            modelBuilder.Entity<DimClasificacion>(entity =>
            {
                entity.ToTable("DimClasificacion", "dbo");
                entity.HasKey(d => d.ClasificacionKey);
            });

            modelBuilder.Entity<DimFecha>(entity =>
            {
                entity.ToTable("DimFecha", "dbo");
                entity.HasKey(d => d.FechaKey);
            });

            modelBuilder.Entity<DimProducto>(entity =>
            {
                entity.ToTable("DimProducto", "dbo");
                entity.HasKey(d => d.ProductoKey);
            });

            // TABLAS DE HECHOS
            modelBuilder.Entity<FactOpinion>(entity =>
            {
                entity.ToTable("FactOpinion", "dbo");
                entity.HasKey(f => f.OpinionKey);
            });

            modelBuilder.Entity<FactOpinionDiarium>(entity =>
            {
                entity.ToTable("FactOpinionDiarium", "dbo");
                entity.HasKey(f => f.ResumenKey);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}