using Microsoft.EntityFrameworkCore;
using OpinionsAnalytics.Domain.Entities.Db;

namespace OpinionsAnalytics.Persistence.Context
{
    public class ResenasContext : DbContext
    {
        public ResenasContext(DbContextOptions<ResenasContext> options) : base(options)
        {
        }

        public DbSet<Resenas> Resenas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Asegura el mapeo consistente con la entidad Resenas.
            // Ajusta schema si tu BD usa "opinionesClientes" o "dbo".
            modelBuilder.Entity<Resenas>(entity =>
            {
                entity.ToTable("webReviews", "dbo");
                entity.HasKey(e => e.IdReview);
                entity.Property(e => e.IdReview).HasColumnName("idReview");
                entity.Property(e => e.IdCliente).HasColumnName("idCliente");
                entity.Property(e => e.IdProducto).HasColumnName("idProducto");
                entity.Property(e => e.Fecha).HasColumnName("fecha");
                entity.Property(e => e.Comentario).HasColumnName("comentario");
                entity.Property(e => e.Rating).HasColumnName("rating");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}