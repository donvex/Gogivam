using Microsoft.EntityFrameworkCore;
using Deliveries.Domain.Entities;
using Deliveries.Infrastructure.Persistence.Configurations;

namespace Deliveries.Infrastructure.Persistence;

/// <summary>
/// Contexte Entity Framework pour la base de données des livraisons
/// </summary>
public class DeliveryDbContext : DbContext
{
    public DbSet<Delivery> Deliveries { get; set; } = null!;

    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Appliquer les configurations
        modelBuilder.ApplyConfiguration(new DeliveryConfiguration());

        // Configuration globale des types
        ConfigureGlobalTypes(modelBuilder);
    }

    private void ConfigureGlobalTypes(ModelBuilder modelBuilder)
    {
        // Configuration des propriétés de type string
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    // Limiter la taille par défaut des strings pour PostgreSQL
                    if (property.GetMaxLength() == null)
                    {
                        property.SetMaxLength(500);
                    }
                }
            }
        }

        // Configuration des enums pour PostgreSQL
        modelBuilder.HasPostgresEnum<DeliveryStatus>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Mettre à jour les timestamps automatiquement
        var entries = ChangeTracker.Entries<AggregateRoot>();
        
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Property(nameof(AggregateRoot.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}