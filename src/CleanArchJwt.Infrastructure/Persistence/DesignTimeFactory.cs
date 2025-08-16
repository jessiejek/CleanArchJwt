using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CleanArchJwt.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Point to WebApi project where appsettings.json is
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../CleanArchJwt.WebApi");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlServer(connectionString);

            return new AppDbContext(builder.Options);
        }
    }
}
