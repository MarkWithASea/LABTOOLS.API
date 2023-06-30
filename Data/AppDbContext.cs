using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        private bool configured;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            configured = true;
        }

        public AppDbContext(string connectionString)
        {
            configured = false;
            
            _connectionString = connectionString;
        }

        public AppDbContext(IHttpContextAccessor httpContextAccessor, string connectionString)
        {
            configured = false;

            _httpContextAccessor = httpContextAccessor;
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!configured)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

        public DbSet<SeedingEntry> SeedingEntries { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; init; }

        public DbSet<Models.Analyzer> Analyzers { get; set; }
    }
}