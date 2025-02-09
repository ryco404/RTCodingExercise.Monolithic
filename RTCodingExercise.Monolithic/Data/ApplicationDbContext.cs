using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Plate> Plates { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plate>().Property(p => p.CreatedOnUtc)
                                        .HasDefaultValueSql("GETUTCDATE()");

            base.OnModelCreating(modelBuilder);
        }
    }
}
