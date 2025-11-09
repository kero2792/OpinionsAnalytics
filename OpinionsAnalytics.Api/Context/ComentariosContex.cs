using Microsoft.EntityFrameworkCore;
using OpinionsAnalytics.Api.Entities;

namespace OpinionsAnalytics.Api.Context
{
    public class ComentariosContex : DbContext
    {
        public ComentariosContex(DbContextOptions<ComentariosContex> options) : base(options)
        {
        }

        public DbSet<ViewComentarios> ViewComentarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewComentarios>(entity =>
            {
                entity.ToView("comentarios", "dbo");
                entity.HasKey(e => e.IdReview);

                entity.Property(e => e.IdReview).HasColumnName("idReview");
                entity.Property(e => e.IdCliente).HasColumnName("idCliente");
                entity.Property(e => e.NombreCliente).HasColumnName("nombreCliente");
                entity.Property(e => e.IdProducto).HasColumnName("idProducto");
                entity.Property(e => e.NombreProducto).HasColumnName("nombreProducto");
                entity.Property(e => e.Comentario).HasColumnName("comentario");
                entity.Property(e => e.Rating).HasColumnName("rating");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}