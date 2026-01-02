using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Deliveries.Infrastructure.Persistence;
using System.IO;

namespace Deliveries.Infrastructure.Persistence.Factories;

/// <summary>
/// Factory pour créer le DbContext au moment du design (pour les migrations)
/// </summary>
public class DeliveryDbContextFactory : IDesignTimeDbContextFactory<DeliveryDbContext>
{
    public DeliveryDbContext CreateDbContext(string[] args)
    {
        // Configuration pour les migrations
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<DeliveryDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=deliveries_db;Username=admin;Password=admin;";

        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(DeliveryDbContext).Assembly.FullName);
        });

        return new DeliveryDbContext(optionsBuilder.Options);
    }
}