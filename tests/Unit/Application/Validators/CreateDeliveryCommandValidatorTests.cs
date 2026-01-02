using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;
using Deliveries.Application.Validators;
using Deliveries.Application.Commands.Deliveries;
using Deliveries.Domain.DTOs;
using Deliveries.Domain.Constants;

namespace Deliveries.Unit.Tests.Application.Validators;

/// <summary>
/// Tests unitaires pour CreateDeliveryCommandValidator
/// </summary>
public class CreateDeliveryCommandValidatorTests
{
    private readonly CreateDeliveryCommandValidator _validator;

    public CreateDeliveryCommandValidatorTests()
    {
        _validator = new CreateDeliveryCommandValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyClientName_ShouldHaveValidationError(string clientName)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { ClientName = clientName };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientName)
              .WithErrorMessage("Le nom du client est obligatoire");
    }

    [Theory]
    [InlineData("A")]  // Too short
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")] // Too long
    public void Validate_ClientNameWrongLength_ShouldHaveValidationError(string clientName)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { ClientName = clientName };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientName)
              .WithErrorMessage($"Le nom du client doit contenir entre {DeliveryConstants.MinClientNameLength} et {DeliveryConstants.MaxClientNameLength} caractères");
    }

    [Theory]
    [InlineData("John Doe")]
    [InlineData("Marie-Claire Dubois")]
    [InlineData("Jean Pierre")]
    public void Validate_ValidClientName_ShouldNotHaveValidationError(string clientName)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { ClientName = clientName };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ClientName);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void Validate_InvalidWeight_ShouldHaveValidationError(double weight)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { Weight = weight };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }

    [Theory]
    [InlineData(1001.0)] // Above max
    public void Validate_WeightTooHigh_ShouldHaveValidationError(double weight)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { Weight = weight };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Weight)
              .WithErrorMessage($"Le poids ne peut pas dépasser {DeliveryConstants.MaxWeight} kg");
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(5.0)]
    [InlineData(100.0)]
    [InlineData(1000.0)]
    public void Validate_ValidWeight_ShouldNotHaveValidationError(double weight)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { Weight = weight };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Weight);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.5)]
    public void Validate_InvalidDistance_ShouldHaveValidationError(double distance)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { Distance = distance };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Distance);
    }

    [Theory]
    [InlineData(10001.0)] // Above max
    public void Validate_DistanceTooHigh_ShouldHaveValidationError(double distance)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { Distance = distance };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Distance)
              .WithErrorMessage($"La distance ne peut pas dépasser {DeliveryConstants.MaxDistance} km");
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(50.0)]
    [InlineData(500.0)]
    [InlineData(10000.0)]
    public void Validate_ValidDistance_ShouldNotHaveValidationError(double distance)
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { Distance = distance };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Distance);
    }

    [Fact]
    public void Validate_NullOriginAddress_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { OriginAddress = null };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OriginAddress)
              .WithErrorMessage("L'adresse d'origine est obligatoire");
    }

    [Fact]
    public void Validate_NullDestinationAddress_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateValidCommand();
        command = command with { DestinationAddress = null };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DestinationAddress)
              .WithErrorMessage("L'adresse de destination est obligatoire");
    }

    [Fact]
    public void Validate_SameOriginAndDestination_ShouldHaveValidationError()
    {
        // Arrange
        var sameAddress = new CreateAddressDto
        {
            Street = "123 Rue de Rivoli",
            City = "Paris",
            PostalCode = "75001",
            Country = "France"
        };

        var command = CreateValidCommand();
        command = command with { 
            OriginAddress = sameAddress,
            DestinationAddress = sameAddress
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("L'adresse d'origine et de destination ne peuvent pas être identiques");
    }

    [Fact]
    public void Validate_DifferentAddresses_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = CreateValidCommand(); // Already has different addresses

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPassAllValidations()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateDeliveryCommand CreateValidCommand()
    {
        return new CreateDeliveryCommand
        {
            ClientName = "Jean Dupont",
            Weight = 5.0,
            Distance = 100.0,
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
    }
}