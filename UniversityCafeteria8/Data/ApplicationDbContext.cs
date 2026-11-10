using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteria8.Models;

namespace UniversityCafeteria8.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet <Product> Products { get; set; }
        public DbSet<SupplyRequest> SupplyRequests { get; set; }
        public DbSet <RequestItem> RequestItems { get; set; }
    }
}
