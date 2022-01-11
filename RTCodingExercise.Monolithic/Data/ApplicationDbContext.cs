using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Plate> Plates { get; set; }
    }
}