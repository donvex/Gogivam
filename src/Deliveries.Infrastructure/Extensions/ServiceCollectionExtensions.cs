using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Deliveries.Domain.Entities;
using Deliveries.Infrastructure.Persistence;
using Deliveries.Infrastructure.Persistence.Seeders;
using Deliveries.Infrastructure.Repositories;
using Deliveries.Infrastructure.Services;
using Deliveries.Infrastructure.HealthChecks;
using Deliveries.Infrastructure.Data;

namespace Deliveries.Infrastructure.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services d'infrastructure
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre tous les services d'infrastructure
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configuration de la base de données PostgreSQL
        AddDatabase(services, configuration);

        // Enregistrement des repositories
        AddRepositories(services);

        // Services d'infrastructure
        AddInfrastructureSpecificServices(services);

        return services;
    }

    /// <summary>
    /// Configure la base de données PostgreSQL
    /// </summary>
    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=deliveries_db;Username=admin;Password=admin;";

        services.AddDbContext<DeliveryDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(DeliveryDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });

            // Configuration pour le développement
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        // Health checks pour PostgreSQL
        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db", "ready" })
            .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "db" })
            .AddCheck<DatabaseMigrationHealthCheck>("database_migrations", tags: new[] { "migrations" });
    }

    /// <summary>
    /// Enregistre les repositories
    /// </summary>
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
    }

    /// <summary>
    /// Enregistre les services spécifiques à l'infrastructure
    /// </summary>
    private static void AddInfrastructureSpecificServices(IServiceCollection services)
    {
        // Connection factory
        services.AddScoped<IDbConnectionFactory, PostgreSQLConnectionFactory>();

        // Seeder pour les données initiales
        services.AddScoped<DeliveryDbSeeder>();

        // Service d'initialisation de la base de données
        services.AddHostedService<DatabaseInitializationService>();

        // Health checks personnalisés
        services.AddScoped<DatabaseHealthCheck>();
        services.AddScoped<DatabaseMigrationHealthCheck>();
    }

    /// <summary>
    /// Configure les options spécifiques à PostgreSQL
    /// </summary>
    public static IServiceCollection AddPostgreSQLConfiguration(this IServiceCollection services)
    {
        // Configuration globale pour PostgreSQL et les enums
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return services;
    }
}