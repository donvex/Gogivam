using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Deliveries.Infrastructure.Data;

/// <summary>
/// Factory pour créer des connexions à la base de données PostgreSQL
/// </summary>
public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    string GetConnectionString();
}

/// <summary>
/// Implémentation de la factory de connexions PostgreSQL
/// </summary>
public class PostgreSQLConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    private readonly ILogger<PostgreSQLConnectionFactory> _logger;

    public PostgreSQLConnectionFactory(IConfiguration configuration, ILogger<PostgreSQLConnectionFactory> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=deliveries_db;Username=admin;Password=admin;";
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating new PostgreSQL connection");

            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            _logger.LogDebug("PostgreSQL connection opened successfully");

            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create PostgreSQL connection");
            throw;
        }
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}

/// <summary>
/// Configuration pour la base de données
/// </summary>
public class DatabaseConfiguration
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = string.Empty;
    public bool EnableDetailedErrors { get; set; } = false;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public int CommandTimeout { get; set; } = 30;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelay { get; set; } = 5;
}