using Xunit;
using FluentAssertions;
using Deliveries.Application.Services;
using Deliveries.Domain.Constants;

namespace Deliveries.Unit.Tests.Application.Services;

/// <summary>
/// Tests unitaires pour PricingService
/// </summary>
public class PricingServiceTests
{
    [Theory]
    [InlineData(1.0, 10.0, 5.0 + 1.0 + 5.0)] // Base + Weight + Distance
    [InlineData(5.0, 100.0, 5.0 + 5.0 + 50.0)]
    [InlineData(0.5, 1.0, 5.0 + 0.5 + 0.5)]
    [InlineData(10.0, 200.0, 5.0 + 10.0 + 100.0)]
    public void CalculatePrice_ValidInputs_ShouldCalculateCorrectly(
        double weight, double distance, double expectedTotal)
    {
        // Arrange
        var service = new PricingService();

        // Act
        var result = service.CalculatePrice(weight, distance);

        // Assert
        result.Should().NotBeNull();
        result.BasePrice.Should().Be(PricingConstants.DefaultBasePrice);
        result.WeightPrice.Should().Be((decimal)weight * PricingConstants.DefaultPricePerKg);
        result.DistancePrice.Should().Be((decimal)distance * PricingConstants.DefaultPricePerKm);
        result.TotalPrice.Should().Be((decimal)expectedTotal);
        result.Currency.Should().Be(PricingConstants.DefaultCurrency);
    }

    [Theory]
    [InlineData(2.0, 5.0, 10.0, 20.0)] // 2 + 5*2 + 5*2 = 22
    [InlineData(1.0, 2.0, 3.0, 11.0)]  // 1 + 2*2 + 5*2 = 15
    public void CalculatePrice_CustomRates_ShouldCalculateCorrectly(
        decimal basePrice, decimal pricePerKg, decimal pricePerKm, double expectedTotal)
    {
        // Arrange
        var service = new PricingService(basePrice, pricePerKm, pricePerKg);

        // Act
        var result = service.CalculatePrice(2.0, 5.0);

        // Assert
        result.BasePrice.Should().Be(basePrice);
        result.WeightPrice.Should().Be(pricePerKg * 2.0m);
        result.DistancePrice.Should().Be(pricePerKm * 5.0m);
        result.TotalPrice.Should().Be((decimal)expectedTotal);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void CalculatePrice_InvalidWeight_ShouldThrowArgumentException(double weight)
    {
        // Arrange
        var service = new PricingService();

        // Act & Assert
        var action = () => service.CalculatePrice(weight, 100.0);
        action.Should().Throw<ArgumentException>()
              .WithMessage("Le poids doit être positif*")
              .And.ParamName.Should().Be("weight");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void CalculatePrice_InvalidDistance_ShouldThrowArgumentException(double distance)
    {
        // Arrange
        var service = new PricingService();

        // Act & Assert
        var action = () => service.CalculatePrice(5.0, distance);
        action.Should().Throw<ArgumentException>()
              .WithMessage("La distance doit être positive*")
              .And.ParamName.Should().Be("distance");
    }

    [Fact]
    public void CalculatePrice_VerySmallAmounts_ShouldApplyMinimumPrice()
    {
        // Arrange
        var service = new PricingService(0.10m, 0.01m, 0.01m); // Very low prices
        
        // Act
        var result = service.CalculatePrice(0.1, 0.1); // 0.10 + 0.001 + 0.001 = 0.102

        // Assert
        result.TotalPrice.Should().Be(PricingConstants.MinimumPrice);
        result.BasePrice.Should().Be(PricingConstants.MinimumPrice);
        result.WeightPrice.Should().Be(0);
        result.DistancePrice.Should().Be(0);
    }

    [Fact]
    public void CalculatePrice_EdgeCaseMinimumBoundary_ShouldWorkCorrectly()
    {
        // Arrange
        var service = new PricingService();
        
        // Act - Very small values that should still give total > minimum
        var result = service.CalculatePrice(0.1, 0.1); // 5 + 0.1 + 0.05 = 5.15

        // Assert
        result.TotalPrice.Should().BeGreaterThan(PricingConstants.MinimumPrice);
        result.BasePrice.Should().Be(5.0m);
        result.WeightPrice.Should().Be(0.1m);
        result.DistancePrice.Should().Be(0.05m);
    }

    [Theory]
    [InlineData(1000.0, 5000.0)] // Heavy and long distance
    [InlineData(0.01, 0.01)]     // Very light and short
    [InlineData(50.0, 1500.0)]   // Medium values
    public void CalculatePrice_ExtremeCases_ShouldHandleGracefully(double weight, double distance)
    {
        // Arrange
        var service = new PricingService();

        // Act
        var result = service.CalculatePrice(weight, distance);

        // Assert
        result.Should().NotBeNull();
        result.TotalPrice.Should().BeGreaterOrEqualTo(PricingConstants.MinimumPrice);
        result.BasePrice.Should().BeGreaterOrEqualTo(0);
        result.WeightPrice.Should().BeGreaterOrEqualTo(0);
        result.DistancePrice.Should().BeGreaterOrEqualTo(0);
        result.Currency.Should().Be(PricingConstants.DefaultCurrency);
    }

    [Fact]
    public void CalculatePrice_DefaultConstants_ShouldUseCorrectValues()
    {
        // Arrange
        var service = new PricingService();

        // Act
        var result = service.CalculatePrice(1.0, 1.0);

        // Assert
        result.BasePrice.Should().Be(5.0m); // Default base price
        result.WeightPrice.Should().Be(1.0m); // 1 kg * 1 EUR/kg
        result.DistancePrice.Should().Be(0.5m); // 1 km * 0.5 EUR/km
        result.TotalPrice.Should().Be(6.5m);
        result.Currency.Should().Be("EUR");
    }
}