using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data
{
    public class AppDbContext : DbContext
    {
        //private readonly string _connectionString;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<SeedingEntry> SeedingEntries { get; set; }

        public DbSet<User> Users { get; set; }
    }
}