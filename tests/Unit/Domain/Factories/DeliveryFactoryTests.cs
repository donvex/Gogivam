using Xunit;
using FluentAssertions;
using Moq;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Factories;

namespace Deliveries.Unit.Tests.Domain.Factories;

/// <summary>
/// Tests unitaires pour DeliveryFactory
/// </summary>
public class DeliveryFactoryTests
{
    private readonly Mock<IPricingService> _mockPricingService;
    private readonly DeliveryFactory _factory;

    public DeliveryFactoryTests()
    {
        _mockPricingService = new Mock<IPricingService>();
        _factory = new DeliveryFactory(_mockPricingService.Object);
    }

    [Fact]
    public void CreateDelivery_ValidParameters_ShouldCreateDeliveryWithCalculatedPrice()
    {
        // Arrange
        var clientName = "Jean Dupont";
        var originAddress = Address.Create("123 Rue de Rivoli", "Paris", "75001", "France");
        var destinationAddress = Address.Create("456 Avenue des Champs", "Lyon", "69000", "France");
        var weight = 5.0;
        var distance = 100.0;
        var expectedPrice = new Price(5.0m, 5.0m, 50.0m, "EUR");

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Returns(expectedPrice);

        // Act
        var result = _factory.CreateDelivery(clientName, originAddress, destinationAddress, weight, distance);

        // Assert
        result.Should().NotBeNull();
        result.ClientName.Should().Be(clientName);
        result.OriginAddress.Should().Be(originAddress);
        result.DestinationAddress.Should().Be(destinationAddress);
        result.Weight.Should().Be(weight);
        result.Distance.Should().Be(distance);
        result.Price.Should().Be(expectedPrice);
        result.Status.Should().Be(Domain.Constants.DeliveryStatus.Pending);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeNull();

        _mockPricingService.Verify(x => x.CalculatePrice(weight, distance), Times.Once);
    }

    [Theory]
    [InlineData(1.0, 10.0)]
    [InlineData(50.0, 500.0)]
    [InlineData(0.5, 1.5)]
    public void CreateDelivery_DifferentWeightsAndDistances_ShouldCallPricingService(double weight, double distance)
    {
        // Arrange
        var clientName = "Test Client";
        var originAddress = Address.Create("Origin Street", "Origin City", "12345", "Origin Country");
        var destinationAddress = Address.Create("Dest Street", "Dest City", "67890", "Dest Country");
        var expectedPrice = new Price(5.0m, (decimal)weight, (decimal)distance * 0.5m, "EUR");

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Returns(expectedPrice);

        // Act
        var result = _factory.CreateDelivery(clientName, originAddress, destinationAddress, weight, distance);

        // Assert
        result.Weight.Should().Be(weight);
        result.Distance.Should().Be(distance);
        result.Price.Should().Be(expectedPrice);

        _mockPricingService.Verify(x => x.CalculatePrice(weight, distance), Times.Once);
    }

    [Fact]
    public void CreateDelivery_PricingServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var clientName = "Test Client";
        var originAddress = Address.Create("Origin Street", "Origin City", "12345", "Origin Country");
        var destinationAddress = Address.Create("Dest Street", "Dest City", "67890", "Dest Country");
        var weight = 5.0;
        var distance = 100.0;

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Throws(new ArgumentException("Invalid pricing parameters"));

        // Act & Assert
        var action = () => _factory.CreateDelivery(clientName, originAddress, destinationAddress, weight, distance);
        action.Should().Throw<ArgumentException>()
              .WithMessage("Invalid pricing parameters");

        _mockPricingService.Verify(x => x.CalculatePrice(weight, distance), Times.Once);
    }

    [Fact]
    public void CreateDelivery_MultipleCallsWithSameParameters_ShouldCreateDifferentDeliveries()
    {
        // Arrange
        var clientName = "Test Client";
        var originAddress = Address.Create("Origin Street", "Origin City", "12345", "Origin Country");
        var destinationAddress = Address.Create("Dest Street", "Dest City", "67890", "Dest Country");
        var weight = 5.0;
        var distance = 100.0;
        var price = new Price(5.0m, 5.0m, 50.0m, "EUR");

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Returns(price);

        // Act
        var delivery1 = _factory.CreateDelivery(clientName, originAddress, destinationAddress, weight, distance);
        var delivery2 = _factory.CreateDelivery(clientName, originAddress, destinationAddress, weight, distance);

        // Assert
        delivery1.Should().NotBe(delivery2);
        delivery1.Id.Should().NotBe(delivery2.Id);
        
        // But should have same properties
        delivery1.ClientName.Should().Be(delivery2.ClientName);
        delivery1.Weight.Should().Be(delivery2.Weight);
        delivery1.Distance.Should().Be(delivery2.Distance);
        delivery1.Price.Should().Be(delivery2.Price);

        _mockPricingService.Verify(x => x.CalculatePrice(weight, distance), Times.Exactly(2));
    }

    [Fact]
    public void CreateDelivery_NullPricingService_ShouldThrowExceptionOnConstruction()
    {
        // Act & Assert
        var action = () => new DeliveryFactory(null!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("pricingService");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CreateDelivery_InvalidClientName_ShouldThrowArgumentException(string clientName)
    {
        // Arrange
        var originAddress = Address.Create("Origin Street", "Origin City", "12345", "Origin Country");
        var destinationAddress = Address.Create("Dest Street", "Dest City", "67890", "Dest Country");
        var weight = 5.0;
        var distance = 100.0;
        var price = new Price(5.0m, 5.0m, 50.0m, "EUR");

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Returns(price);

        // Act & Assert
        var action = () => _factory.CreateDelivery(clientName, originAddress, destinationAddress, weight, distance);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateDelivery_NullOriginAddress_ShouldThrowArgumentNullException()
    {
        // Arrange
        var clientName = "Test Client";
        var destinationAddress = Address.Create("Dest Street", "Dest City", "67890", "Dest Country");
        var weight = 5.0;
        var distance = 100.0;
        var price = new Price(5.0m, 5.0m, 50.0m, "EUR");

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Returns(price);

        // Act & Assert
        var action = () => _factory.CreateDelivery(clientName, null!, destinationAddress, weight, distance);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateDelivery_NullDestinationAddress_ShouldThrowArgumentNullException()
    {
        // Arrange
        var clientName = "Test Client";
        var originAddress = Address.Create("Origin Street", "Origin City", "12345", "Origin Country");
        var weight = 5.0;
        var distance = 100.0;
        var price = new Price(5.0m, 5.0m, 50.0m, "EUR");

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Returns(price);

        // Act & Assert
        var action = () => _factory.CreateDelivery(clientName, originAddress, null!, weight, distance);
        action.Should().Throw<ArgumentNullException>();
    }
}