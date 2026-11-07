using Microsoft.EntityFrameworkCore;

namespace AcademyGraduationCeremony.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Offer> Offers { get; set; }
    }
}
