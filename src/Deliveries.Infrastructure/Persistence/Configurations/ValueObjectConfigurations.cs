using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Constants;

namespace Deliveries.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework pour le value object Address
/// </summary>
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // Cette configuration ne sera pas utilisée directement car Address est un value object
        // Les configurations réelles sont dans DeliveryConfiguration via OwnsOne
        
        // Cette classe existe pour la cohérence de la structure
        // mais les configurations réelles sont inline dans DeliveryConfiguration
    }
}

/// <summary>
/// Configuration Entity Framework pour le value object Price
/// </summary>
public class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        // Cette configuration ne sera pas utilisée directement car Price est un value object
        // Les configurations réelles sont dans DeliveryConfiguration via OwnsOne
        
        // Cette classe existe pour la cohérence de la structure
        // mais les configurations réelles sont inline dans DeliveryConfiguration
    }
}