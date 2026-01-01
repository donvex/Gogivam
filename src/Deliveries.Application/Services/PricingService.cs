using Deliveries.Domain.Entities;
using Deliveries.Domain.Constants;

namespace Deliveries.Application.Services;

/// <summary>
/// Service de tarification avec règle de base : prix_de_base + distance + poids
/// </summary>
public class PricingService : IPricingService
{
    private readonly decimal _basePrice;
    private readonly decimal _pricePerKm;
    private readonly decimal _pricePerKg;

    public PricingService(
        decimal basePrice = PricingConstants.DefaultBasePrice,
        decimal pricePerKm = PricingConstants.DefaultPricePerKm,
        decimal pricePerKg = PricingConstants.DefaultPricePerKg)
    {
        _basePrice = basePrice;
        _pricePerKm = pricePerKm;
        _pricePerKg = pricePerKg;
    }

    public Price CalculatePrice(double weight, double distance)
    {
        if (weight <= 0) throw new ArgumentException("Le poids doit être positif", nameof(weight));
        if (distance <= 0) throw new ArgumentException("La distance doit être positive", nameof(distance));

        var weightPrice = (decimal)weight * _pricePerKg;
        var distancePrice = (decimal)distance * _pricePerKm;

        var totalCalculated = _basePrice + weightPrice + distancePrice;

        // Appliquer un prix minimum si nécessaire
        if (totalCalculated < PricingConstants.MinimumPrice)
        {
            return new Price(PricingConstants.MinimumPrice, 0, 0, PricingConstants.DefaultCurrency);
        }

        return new Price(_basePrice, weightPrice, distancePrice, PricingConstants.DefaultCurrency);
    }
}