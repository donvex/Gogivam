using Xunit;
using FluentAssertions;
using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using Deliveries.Application.Commands.Deliveries;
using Deliveries.Application.Handlers.Commands;
using Deliveries.Application.Mappers;
using Deliveries.Application.Services;
using Deliveries.Domain.Entities;
using Deliveries.Domain.DTOs;
using Deliveries.Domain.Factories;

namespace Deliveries.Unit.Tests.Handlers.Commands;

/// <summary>
/// Tests unitaires pour CreateDeliveryCommandHandler
/// </summary>
public class CreateDeliveryCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IDeliveryRepository> _mockRepository;
    private readonly Mock<IPricingService> _mockPricingService;
    private readonly Mock<DeliveryFactory> _mockFactory;
    private readonly Mock<ILogger<CreateDeliveryCommandHandler>> _mockLogger;
    private readonly CreateDeliveryCommandHandler _handler;

    public CreateDeliveryCommandHandlerTests()
    {
        _fixture = new Fixture();
        _mockRepository = new Mock<IDeliveryRepository>();
        _mockPricingService = new Mock<IPricingService>();
        _mockFactory = new Mock<DeliveryFactory>(_mockPricingService.Object);
        _mockLogger = new Mock<ILogger<CreateDeliveryCommandHandler>>();

        _handler = new CreateDeliveryCommandHandler(
            _mockRepository.Object,
            _mockPricingService.Object,
            _mockFactory.Object,
            _mockLogger.Object);
    }

    [Theory, AutoData]
    public async Task Handle_ValidCommand_ShouldCreateDeliverySuccessfully(
        string clientName,
        double weight,
        double distance)
    {
        // Arrange
        var command = new CreateDeliveryCommand
        {
            ClientName = clientName,
            Weight = weight,
            Distance = distance,
            OriginAddress = new CreateAddressDto
            {
                Street = "123 Rue de Rivoli",
                City = "Paris",
                PostalCode = "75001",
                Country = "France"
            },
            DestinationAddress = new CreateAddressDto
            {
                Street = "456 Avenue des Champs",
                City = "Lyon",
                PostalCode = "69000",
                Country = "France"
            }
        };

        var expectedPrice = new Price(5.0m, 10.0m, 25.0m, "EUR");
        var expectedDelivery = Delivery.Create(
            clientName,
            Address.Create(command.OriginAddress.Street, command.OriginAddress.City, 
                         command.OriginAddress.PostalCode, command.OriginAddress.Country),
            Address.Create(command.DestinationAddress.Street, command.DestinationAddress.City,
                         command.DestinationAddress.PostalCode, command.DestinationAddress.Country),
            weight,
            distance,
            expectedPrice);

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Returns(expectedPrice);

        _mockFactory.Setup(x => x.CreateDelivery(
                It.Is<string>(s => s == clientName),
                It.IsAny<Address>(),
                It.IsAny<Address>(),
                It.Is<double>(w => w == weight),
                It.Is<double>(d => d == distance)))
                  .Returns(expectedDelivery);

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedDelivery);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ClientName.Should().Be(clientName);
        result.Weight.Should().Be(weight);
        result.Distance.Should().Be(distance);
        result.Status.Should().Be("Pending");
        result.Price.Should().NotBeNull();
        result.Price.TotalPrice.Should().Be(40.0m); // 5 + 10 + 25

        _mockPricingService.Verify(x => x.CalculatePrice(weight, distance), Times.Once);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldMapAddressesCorrectly()
    {
        // Arrange
        var command = new CreateDeliveryCommand
        {
            ClientName = "Test Client",
            Weight = 5.0,
            Distance = 100.0,
            OriginAddress = new CreateAddressDto
            {
                Street = "123 Test Street",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            },
            DestinationAddress = new CreateAddressDto
            {
                Street = "456 Dest Street",
                City = "Dest City",
                PostalCode = "67890",
                Country = "Dest Country"
            }
        };

        var price = new Price(5.0m, 5.0m, 50.0m, "EUR");
        var delivery = Delivery.Create(
            command.ClientName,
            Address.Create(command.OriginAddress.Street, command.OriginAddress.City,
                         command.OriginAddress.PostalCode, command.OriginAddress.Country),
            Address.Create(command.DestinationAddress.Street, command.DestinationAddress.City,
                         command.DestinationAddress.PostalCode, command.DestinationAddress.Country),
            command.Weight,
            command.Distance,
            price);

        _mockPricingService.Setup(x => x.CalculatePrice(It.IsAny<double>(), It.IsAny<double>()))
                          .Returns(price);
        _mockFactory.Setup(x => x.CreateDelivery(It.IsAny<string>(), It.IsAny<Address>(), 
                                                It.IsAny<Address>(), It.IsAny<double>(), It.IsAny<double>()))
                  .Returns(delivery);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(delivery);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.OriginAddress.Street.Should().Be("123 Test Street");
        result.OriginAddress.City.Should().Be("Test City");
        result.OriginAddress.PostalCode.Should().Be("12345");
        result.OriginAddress.Country.Should().Be("Test Country");

        result.DestinationAddress.Street.Should().Be("456 Dest Street");
        result.DestinationAddress.City.Should().Be("Dest City");
        result.DestinationAddress.PostalCode.Should().Be("67890");
        result.DestinationAddress.Country.Should().Be("Dest Country");
    }

    [Theory]
    [InlineData(0.0, 100.0)]
    [InlineData(-1.0, 100.0)]
    [InlineData(5.0, 0.0)]
    [InlineData(5.0, -1.0)]
    public async Task Handle_InvalidWeightOrDistance_ShouldThrowArgumentException(
        double weight, double distance)
    {
        // Arrange
        var command = new CreateDeliveryCommand
        {
            ClientName = "Test Client",
            Weight = weight,
            Distance = distance,
            OriginAddress = new CreateAddressDto
            {
                Street = "123 Test Street",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            },
            DestinationAddress = new CreateAddressDto
            {
                Street = "456 Dest Street",
                City = "Dest City",
                PostalCode = "67890",
                Country = "Dest Country"
            }
        };

        _mockPricingService.Setup(x => x.CalculatePrice(weight, distance))
                          .Throws<ArgumentException>();

        // Act & Assert
        var action = () => _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<ArgumentException>();

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateDeliveryCommand
        {
            ClientName = "Test Client",
            Weight = 5.0,
            Distance = 100.0,
            OriginAddress = new CreateAddressDto
            {
                Street = "123 Test Street",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            },
            DestinationAddress = new CreateAddressDto
            {
                Street = "456 Dest Street",
                City = "Dest City",
                PostalCode = "67890",
                Country = "Dest Country"
            }
        };

        var price = new Price(5.0m, 5.0m, 50.0m, "EUR");
        var delivery = Delivery.Create(
            command.ClientName,
            Address.Create(command.OriginAddress.Street, command.OriginAddress.City,
                         command.OriginAddress.PostalCode, command.OriginAddress.Country),
            Address.Create(command.DestinationAddress.Street, command.DestinationAddress.City,
                         command.DestinationAddress.PostalCode, command.DestinationAddress.Country),
            command.Weight,
            command.Distance,
            price);

        _mockPricingService.Setup(x => x.CalculatePrice(It.IsAny<double>(), It.IsAny<double>()))
                          .Returns(price);
        _mockFactory.Setup(x => x.CreateDelivery(It.IsAny<string>(), It.IsAny<Address>(),
                                                It.IsAny<Address>(), It.IsAny<double>(), It.IsAny<double>()))
                  .Returns(delivery);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var action = () => _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("Database error");
    }
}