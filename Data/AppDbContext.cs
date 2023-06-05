using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public AppDbContext(IHttpContextAccessor httpContextAccessor, string connectionString)
        {
            _httpContextAccessor = httpContextAccessor;
            _connectionString = connectionString;
        }

        public DbSet<SeedingEntry> SeedingEntries { get; set; }

        public DbSet<User> Users { get; set; }
    }
}