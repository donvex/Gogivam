using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Constants;

namespace Deliveries.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Delivery
/// </summary>
public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        // Table
        builder.ToTable("deliveries");

        // Clé primaire
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .IsRequired();

        // Propriétés simples
        builder.Property(d => d.ClientName)
            .HasColumnName("client_name")
            .HasMaxLength(DeliveryConstants.MaxClientNameLength)
            .IsRequired();

        builder.Property(d => d.Weight)
            .HasColumnName("weight")
            .HasColumnType("decimal(8,2)")
            .IsRequired();

        builder.Property(d => d.Distance)
            .HasColumnName("distance")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at");

        // Configuration des value objects
        ConfigureOriginAddress(builder);
        ConfigureDestinationAddress(builder);
        ConfigurePrice(builder);

        // Index
        builder.HasIndex(d => d.ClientName)
            .HasDatabaseName("ix_deliveries_client_name");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("ix_deliveries_status");

        builder.HasIndex(d => d.CreatedAt)
            .HasDatabaseName("ix_deliveries_created_at");

        // Ignorer les événements du domaine pour la persistance
        builder.Ignore(d => d.DomainEvents);
    }

    private void ConfigureOriginAddress(EntityTypeBuilder<Delivery> builder)
    {
        builder.OwnsOne(d => d.OriginAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("origin_street")
                .HasMaxLength(DeliveryConstants.MaxAddressFieldLength)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("origin_city")
                .HasMaxLength(DeliveryConstants.MaxAddressFieldLength)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("origin_postal_code")
                .HasMaxLength(DeliveryConstants.MaxPostalCodeLength)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("origin_country")
                .HasMaxLength(DeliveryConstants.MaxCountryLength)
                .IsRequired();
        });
    }

    private void ConfigureDestinationAddress(EntityTypeBuilder<Delivery> builder)
    {
        builder.OwnsOne(d => d.DestinationAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("destination_street")
                .HasMaxLength(DeliveryConstants.MaxAddressFieldLength)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("destination_city")
                .HasMaxLength(DeliveryConstants.MaxAddressFieldLength)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("destination_postal_code")
                .HasMaxLength(DeliveryConstants.MaxPostalCodeLength)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("destination_country")
                .HasMaxLength(DeliveryConstants.MaxCountryLength)
                .IsRequired();
        });
    }

    private void ConfigurePrice(EntityTypeBuilder<Delivery> builder)
    {
        builder.OwnsOne(d => d.CalculatedPrice, price =>
        {
            price.Property(p => p.BasePrice)
                .HasColumnName("price_base")
                .HasColumnType("decimal(10,2)");

            price.Property(p => p.WeightPrice)
                .HasColumnName("price_weight")
                .HasColumnType("decimal(10,2)");

            price.Property(p => p.DistancePrice)
                .HasColumnName("price_distance")
                .HasColumnType("decimal(10,2)");

            price.Property(p => p.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .HasDefaultValue("EUR");

            // Propriété calculée pour le prix total (non mappée en base)
            price.Ignore(p => p.TotalPrice);
        });
    }
}