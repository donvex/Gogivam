using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Deliveries.Infrastructure.Persistence;
using Deliveries.Infrastructure.Persistence.Seeders;

namespace Deliveries.Infrastructure.Services;

/// <summary>
/// Service d'initialisation de la base de données
/// </summary>
public class DatabaseInitializationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseInitializationService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting database initialization...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var seeder = scope.ServiceProvider.GetRequiredService<DeliveryDbSeeder>();

            // Assurer que la base de données est créée
            await context.Database.EnsureCreatedAsync(cancellationToken);
            
            // Appliquer les migrations si nécessaire
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
                await context.Database.MigrateAsync(cancellationToken);
            }

            // Seeder les données initiales
            await seeder.SeedAsync();

            _logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}