using Xunit;
using FluentAssertions;
using AutoFixture;
using AutoFixture.Xunit2;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Constants;

namespace Deliveries.Unit.Tests.Domain.Entities;

/// <summary>
/// Tests unitaires pour l'entité Delivery
/// </summary>
public class DeliveryTests
{
    private readonly IFixture _fixture;

    public DeliveryTests()
    {
        _fixture = new Fixture();
    }

    [Theory, AutoData]
    public void Create_ValidParameters_ShouldCreateDeliverySuccessfully(
        string clientName,
        double weight,
        double distance)
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act
        var delivery = Delivery.Create(clientName, originAddress, destinationAddress, weight, distance, price);

        // Assert
        delivery.Should().NotBeNull();
        delivery.Id.Should().NotBeEmpty();
        delivery.ClientName.Should().Be(clientName);
        delivery.OriginAddress.Should().Be(originAddress);
        delivery.DestinationAddress.Should().Be(destinationAddress);
        delivery.Weight.Should().Be(weight);
        delivery.Distance.Should().Be(distance);
        delivery.Price.Should().Be(price);
        delivery.Status.Should().Be(DeliveryStatus.Pending);
        delivery.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        delivery.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_InvalidClientName_ShouldThrowArgumentException(string clientName)
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act & Assert
        var action = () => Delivery.Create(clientName, originAddress, destinationAddress, 5.0, 100.0, price);
        action.Should().Throw<ArgumentException>()
              .WithMessage("Le nom du client ne peut pas être vide*");
    }

    [Fact]
    public void Create_NullOriginAddress_ShouldThrowArgumentNullException()
    {
        // Arrange
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act & Assert
        var action = () => Delivery.Create("Test Client", null, destinationAddress, 5.0, 100.0, price);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("originAddress");
    }

    [Fact]
    public void Create_NullDestinationAddress_ShouldThrowArgumentNullException()
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act & Assert
        var action = () => Delivery.Create("Test Client", originAddress, null, 5.0, 100.0, price);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("destinationAddress");
    }

    [Fact]
    public void Create_NullPrice_ShouldThrowArgumentNullException()
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");

        // Act & Assert
        var action = () => Delivery.Create("Test Client", originAddress, destinationAddress, 5.0, 100.0, null);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("price");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void Create_InvalidWeight_ShouldThrowArgumentException(double weight)
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act & Assert
        var action = () => Delivery.Create("Test Client", originAddress, destinationAddress, weight, 100.0, price);
        action.Should().Throw<ArgumentException>()
              .WithMessage("Le poids doit être supérieur à 0*");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void Create_InvalidDistance_ShouldThrowArgumentException(double distance)
    {
        // Arrange
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        // Act & Assert
        var action = () => Delivery.Create("Test Client", originAddress, destinationAddress, 5.0, distance, price);
        action.Should().Throw<ArgumentException>()
              .WithMessage("La distance doit être supérieure à 0*");
    }

    [Fact]
    public void UpdateStatus_ValidStatus_ShouldUpdateSuccessfully()
    {
        // Arrange
        var delivery = CreateValidDelivery();
        var originalUpdatedAt = delivery.UpdatedAt;

        // Act
        delivery.UpdateStatus(DeliveryStatus.InTransit);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.InTransit);
        delivery.UpdatedAt.Should().NotBe(originalUpdatedAt);
        delivery.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStatus_SameStatus_ShouldNotUpdateTimestamp()
    {
        // Arrange
        var delivery = CreateValidDelivery();
        var originalUpdatedAt = delivery.UpdatedAt;

        // Act
        delivery.UpdateStatus(DeliveryStatus.Pending);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Pending);
        delivery.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Theory]
    [InlineData(DeliveryStatus.InTransit)]
    [InlineData(DeliveryStatus.Delivered)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void UpdateStatus_AllValidStatuses_ShouldWork(DeliveryStatus newStatus)
    {
        // Arrange
        var delivery = CreateValidDelivery();

        // Act
        delivery.UpdateStatus(newStatus);

        // Assert
        delivery.Status.Should().Be(newStatus);
        delivery.UpdatedAt.Should().NotBeNull();
    }

    private Delivery CreateValidDelivery()
    {
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var price = new Price(5.0m, 10.0m, 25.0m, "EUR");

        return Delivery.Create("Test Client", originAddress, destinationAddress, 5.0, 100.0, price);
    }
}