using Xunit;
using FluentAssertions;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Constants;

namespace Deliveries.Unit.Tests.Domain.Entities;

/// <summary>
/// Tests unitaires pour l'entité Price
/// </summary>
public class PriceTests
{
    [Theory]
    [InlineData(5.0, 10.0, 25.0, "EUR", 40.0)]
    [InlineData(0.0, 0.0, 0.0, "USD", 0.0)]
    [InlineData(2.5, 3.5, 4.0, "GBP", 10.0)]
    public void Create_ValidParameters_ShouldCalculateTotalCorrectly(
        double basePrice, double weightPrice, double distancePrice, string currency, double expectedTotal)
    {
        // Act
        var price = new Price((decimal)basePrice, (decimal)weightPrice, (decimal)distancePrice, currency);

        // Assert
        price.BasePrice.Should().Be((decimal)basePrice);
        price.WeightPrice.Should().Be((decimal)weightPrice);
        price.DistancePrice.Should().Be((decimal)distancePrice);
        price.Currency.Should().Be(currency);
        price.TotalPrice.Should().Be((decimal)expectedTotal);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_InvalidCurrency_ShouldThrowArgumentException(string currency)
    {
        // Act & Assert
        var action = () => new Price(5.0m, 10.0m, 25.0m, currency);
        action.Should().Throw<ArgumentException>()
              .WithMessage("La devise ne peut pas être vide*");
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void Create_NegativeBasePrice_ShouldThrowArgumentException(double basePrice)
    {
        // Act & Assert
        var action = () => new Price((decimal)basePrice, 10.0m, 25.0m, "EUR");
        action.Should().Throw<ArgumentException>()
              .WithMessage("Le prix de base ne peut pas être négatif*");
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void Create_NegativeWeightPrice_ShouldThrowArgumentException(double weightPrice)
    {
        // Act & Assert
        var action = () => new Price(5.0m, (decimal)weightPrice, 25.0m, "EUR");
        action.Should().Throw<ArgumentException>()
              .WithMessage("Le prix du poids ne peut pas être négatif*");
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void Create_NegativeDistancePrice_ShouldThrowArgumentException(double distancePrice)
    {
        // Act & Assert
        var action = () => new Price(5.0m, 10.0m, (decimal)distancePrice, "EUR");
        action.Should().Throw<ArgumentException>()
              .WithMessage("Le prix de la distance ne peut pas être négatif*");
    }

    [Fact]
    public void FormattedPrice_EuroCurrency_ShouldFormatCorrectly()
    {
        // Arrange
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act
        var formatted = price.FormattedPrice;

        // Assert
        formatted.Should().Be("40,00 EUR");
    }

    [Theory]
    [InlineData("USD", "40.00 USD")]
    [InlineData("GBP", "40.00 GBP")]
    [InlineData("CAD", "40.00 CAD")]
    public void FormattedPrice_DifferentCurrencies_ShouldFormatCorrectly(string currency, string expected)
    {
        // Arrange
        var price = new Price(5.0m, 10.0m, 25.0m, currency);

        // Act
        var formatted = price.FormattedPrice;

        // Assert
        formatted.Should().Be(expected);
    }

    [Fact]
    public void Equality_SamePrices_ShouldBeEqual()
    {
        // Arrange
        var price1 = new Price(5.0m, 10.0m, 25.0m, "EUR");
        var price2 = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act & Assert
        price1.Should().Be(price2);
        (price1 == price2).Should().BeTrue();
        (price1 != price2).Should().BeFalse();
    }

    [Fact]
    public void Equality_DifferentPrices_ShouldNotBeEqual()
    {
        // Arrange
        var price1 = new Price(5.0m, 10.0m, 25.0m, "EUR");
        var price2 = new Price(5.0m, 10.0m, 30.0m, "EUR");

        // Act & Assert
        price1.Should().NotBe(price2);
        (price1 == price2).Should().BeFalse();
        (price1 != price2).Should().BeTrue();
    }

    [Fact]
    public void Equality_DifferentCurrencies_ShouldNotBeEqual()
    {
        // Arrange
        var price1 = new Price(5.0m, 10.0m, 25.0m, "EUR");
        var price2 = new Price(5.0m, 10.0m, 25.0m, "USD");

        // Act & Assert
        price1.Should().NotBe(price2);
        (price1 == price2).Should().BeFalse();
        (price1 != price2).Should().BeTrue();
    }
}