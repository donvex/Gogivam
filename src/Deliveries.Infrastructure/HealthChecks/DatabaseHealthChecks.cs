using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Deliveries.Infrastructure.Persistence;

namespace Deliveries.Infrastructure.HealthChecks;

/// <summary>
/// Health check pour vérifier la disponibilité de la base de données
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly DeliveryDbContext _context;

    public DatabaseHealthCheck(DeliveryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Tester la connexion à la base de données
            await _context.Database.CanConnectAsync(cancellationToken);

            // Vérifier que la table des livraisons existe
            var canQueryDeliveries = await _context.Deliveries.AnyAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                ["database"] = "PostgreSQL",
                ["can_connect"] = true,
                ["can_query"] = true
            };

            return HealthCheckResult.Healthy("Database is healthy", data);
        }
        catch (Exception ex)
        {
            var data = new Dictionary<string, object>
            {
                ["database"] = "PostgreSQL",
                ["can_connect"] = false,
                ["error"] = ex.Message
            };

            return HealthCheckResult.Unhealthy("Database is unhealthy", ex, data);
        }
    }
}

/// <summary>
/// Health check pour vérifier la migration de la base de données
/// </summary>
public class DatabaseMigrationHealthCheck : IHealthCheck
{
    private readonly DeliveryDbContext _context;

    public DatabaseMigrationHealthCheck(DeliveryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                ["applied_migrations_count"] = appliedMigrations.Count(),
                ["pending_migrations_count"] = pendingMigrations.Count(),
                ["pending_migrations"] = pendingMigrations.ToArray()
            };

            if (pendingMigrations.Any())
            {
                return HealthCheckResult.Degraded("Database has pending migrations", data);
            }

            return HealthCheckResult.Healthy("Database migrations are up to date", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to check database migration status", ex);
        }
    }
}