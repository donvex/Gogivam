using Xunit;
using FluentAssertions;
using Deliveries.Application.Mappers;
using Deliveries.Domain.Entities;
using Deliveries.Domain.DTOs;

namespace Deliveries.Unit.Tests.Application.Mappers;

/// <summary>
/// Tests unitaires pour DeliveryMapper
/// </summary>
public class DeliveryMapperTests
{
    [Fact]
    public void MapToDeliveryResponseDto_ValidDelivery_ShouldMapCorrectly()
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");
        var delivery = Delivery.Create("Jean Dupont", originAddress, destinationAddress, 5.0, 100.0, price);

        // Act
        var result = DeliveryMapper.MapToDeliveryResponseDto(delivery);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(delivery.Id);
        result.ClientName.Should().Be("Jean Dupont");
        result.Weight.Should().Be(5.0);
        result.Distance.Should().Be(100.0);
        result.Status.Should().Be("Pending");
        result.CreatedAt.Should().Be(delivery.CreatedAt);
        result.UpdatedAt.Should().Be(delivery.UpdatedAt);

        // Origin Address
        result.OriginAddress.Should().NotBeNull();
        result.OriginAddress.Street.Should().Be("123 Rue de Rivoli");
        result.OriginAddress.City.Should().Be("Paris");
        result.OriginAddress.PostalCode.Should().Be("75001");
        result.OriginAddress.Country.Should().Be("France");
        result.OriginAddress.FullAddress.Should().Be("123 Rue de Rivoli, Paris 75001, France");

        // Destination Address
        result.DestinationAddress.Should().NotBeNull();
        result.DestinationAddress.Street.Should().Be("456 Avenue des Champs");
        result.DestinationAddress.City.Should().Be("Lyon");
        result.DestinationAddress.PostalCode.Should().Be("69000");
        result.DestinationAddress.Country.Should().Be("France");
        result.DestinationAddress.FullAddress.Should().Be("456 Avenue des Champs, Lyon 69000, France");

        // Price
        result.Price.Should().NotBeNull();
        result.Price.BasePrice.Should().Be(5.0m);
        result.Price.WeightPrice.Should().Be(10.0m);
        result.Price.DistancePrice.Should().Be(25.0m);
        result.Price.TotalPrice.Should().Be(40.0m);
        result.Price.Currency.Should().Be("EUR");
        result.Price.FormattedPrice.Should().Be("40,00 EUR");
    }

    [Fact]
    public void MapToAddressResponseDto_ValidAddress_ShouldMapCorrectly()
    {
        // Arrange
        var address = Address.Create("123 Test Street", "Test City", "12345", "Test Country");

        // Act
        var result = DeliveryMapper.MapToAddressResponseDto(address);

        // Assert
        result.Should().NotBeNull();
        result.Street.Should().Be("123 Test Street");
        result.City.Should().Be("Test City");
        result.PostalCode.Should().Be("12345");
        result.Country.Should().Be("Test Country");
        result.FullAddress.Should().Be("123 Test Street, Test City 12345, Test Country");
    }

    [Fact]
    public void MapToPriceResponseDto_ValidPrice_ShouldMapCorrectly()
    {
        // Arrange
        var price = new Price(10.0m, 15.0m, 30.0m, "USD");

        // Act
        var result = DeliveryMapper.MapToPriceResponseDto(price);

        // Assert
        result.Should().NotBeNull();
        result.BasePrice.Should().Be(10.0m);
        result.WeightPrice.Should().Be(15.0m);
        result.DistancePrice.Should().Be(30.0m);
        result.TotalPrice.Should().Be(55.0m);
        result.Currency.Should().Be("USD");
        result.FormattedPrice.Should().Be("55.00 USD");
    }

    [Fact]
    public void MapToDeliveryResponseDto_DeliveryWithUpdatedStatus_ShouldMapUpdatedAt()
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");
        var delivery = Delivery.Create("Jean Dupont", originAddress, destinationAddress, 5.0, 100.0, price);
        
        // Update status to set UpdatedAt
        delivery.UpdateStatus(Domain.Constants.DeliveryStatus.InTransit);

        // Act
        var result = DeliveryMapper.MapToDeliveryResponseDto(delivery);

        // Assert
        result.Status.Should().Be("InTransit");
        result.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().Be(delivery.UpdatedAt);
    }

    [Theory]
    [InlineData("EUR", "25,00 EUR")]
    [InlineData("USD", "25.00 USD")]
    [InlineData("GBP", "25.00 GBP")]
    public void MapToPriceResponseDto_DifferentCurrencies_ShouldFormatCorrectly(string currency, string expectedFormat)
    {
        // Arrange
        var price = new Price(5.0m, 10.0m, 10.0m, currency);

        // Act
        var result = DeliveryMapper.MapToPriceResponseDto(price);

        // Assert
        result.FormattedPrice.Should().Be(expectedFormat);
        result.Currency.Should().Be(currency);
        result.TotalPrice.Should().Be(25.0m);
    }

    [Theory]
    [InlineData("", "", "", "")]
    [InlineData("Street", "", "", "")]
    [InlineData("", "City", "", "")]
    [InlineData("", "", "12345", "")]
    [InlineData("", "", "", "Country")]
    public void MapToAddressResponseDto_PartialAddress_ShouldHandleEmptyFields(
        string street, string city, string postalCode, string country)
    {
        // Arrange
        var address = Address.Create(
            string.IsNullOrEmpty(street) ? "Default Street" : street,
            string.IsNullOrEmpty(city) ? "Default City" : city,
            string.IsNullOrEmpty(postalCode) ? "00000" : postalCode,
            string.IsNullOrEmpty(country) ? "Default Country" : country);

        // Act
        var result = DeliveryMapper.MapToAddressResponseDto(address);

        // Assert
        result.Should().NotBeNull();
        result.Street.Should().NotBeNull();
        result.City.Should().NotBeNull();
        result.PostalCode.Should().NotBeNull();
        result.Country.Should().NotBeNull();
        result.FullAddress.Should().NotBeNull();
        result.FullAddress.Should().Contain(",");
    }

    [Fact]
    public void MapToDeliveryResponseDto_ZeroPrices_ShouldMapCorrectly()
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(0.0m, 0.0m, 0.0m, "EUR");
        var delivery = Delivery.Create("Jean Dupont", originAddress, destinationAddress, 0.1, 0.1, price);

        // Act
        var result = DeliveryMapper.MapToDeliveryResponseDto(delivery);

        // Assert
        result.Price.Should().NotBeNull();
        result.Price.BasePrice.Should().Be(0.0m);
        result.Price.WeightPrice.Should().Be(0.0m);
        result.Price.DistancePrice.Should().Be(0.0m);
        result.Price.TotalPrice.Should().Be(0.0m);
        result.Price.FormattedPrice.Should().Be("0,00 EUR");
    }
}