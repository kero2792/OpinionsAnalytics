namespace OpinionsAnalytics.Persistence.Context
{
    using OpinionsAnalytics.Domain.Entities.Db;
    using Microsoft.EntityFrameworkCore;

    public class ResenasContext : DbContext
    {
        public ResenasContext(DbContextOptions<ResenasContext> options) : base(options)
        {
        }

        public DbSet<Resenas> Resenas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resenas>().ToTable("webReviews", "dbo");
            modelBuilder.Entity<Resenas>().HasKey(r => r.IdReview);
            base.OnModelCreating(modelBuilder);
        }
    }
}