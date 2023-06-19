using LABTOOLS.API.Data;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Extensions
{
    public static class DbContextExtensions
    {
        public static async void SeedData(this IApplicationBuilder app, AppSettings appSettings)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var dbContext = new AppDbContext(appSettings.ConnectionString))
                {
                    ApplySeedDataAndMigrations(dbContext);
                }
            }
        }

        private static void ApplySeedDataAndMigrations(AppDbContext context)
        {
            context!.Database.Migrate();

            var assembly = typeof(DbContextExtensions).Assembly;
            var files = assembly.GetManifestResourceNames();

            var executedSeedings = context.SeedingEntries.ToArray();
            var filePrefix = $"{assembly.GetName().Name}.Seedings";
            foreach (var file in files.Where(f => f.StartsWith(filePrefix) && f.EndsWith(".sql"))
                                      .Select(f => new
                                      {
                                          PhysicalFile = f,
                                          LogicalFile = f.Replace(filePrefix, string.Empty),
                                      })
                                      .OrderBy(f => f.LogicalFile))
            {
                if (executedSeedings.Any(e => e.Name == file.LogicalFile))
                {
                    continue;
                }

                string command = string.Empty;
                using (Stream stream = assembly.GetManifestResourceStream(file.PhysicalFile)!)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        command = reader.ReadToEnd();
                    }
                }

                if (string.IsNullOrWhiteSpace(command))
                {
                    continue;
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Database.ExecuteSqlRaw(command);
                        context.SeedingEntries.Add(new SeedingEntry() { Name = file.LogicalFile });
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
