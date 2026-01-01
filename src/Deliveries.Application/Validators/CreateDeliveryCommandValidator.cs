using FluentValidation;
using Deliveries.Application.Commands.Deliveries;
using Deliveries.Domain.Constants;

namespace Deliveries.Application.Validators;

/// <summary>
/// Validateur FluentValidation pour la commande de création de livraison
/// </summary>
public class CreateDeliveryCommandValidator : AbstractValidator<CreateDeliveryCommand>
{
    public CreateDeliveryCommandValidator()
    {
        RuleFor(x => x.ClientName)
            .NotEmpty()
            .WithMessage("Le nom du client est obligatoire")
            .Length(DeliveryConstants.MinClientNameLength, DeliveryConstants.MaxClientNameLength)
            .WithMessage($"Le nom du client doit contenir entre {DeliveryConstants.MinClientNameLength} et {DeliveryConstants.MaxClientNameLength} caractères");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(DeliveryConstants.MinWeight)
            .WithMessage($"Le poids doit être supérieur ou égal à {DeliveryConstants.MinWeight} kg")
            .LessThanOrEqualTo(DeliveryConstants.MaxWeight)
            .WithMessage($"Le poids ne peut pas dépasser {DeliveryConstants.MaxWeight} kg");

        RuleFor(x => x.Distance)
            .GreaterThanOrEqualTo(DeliveryConstants.MinDistance)
            .WithMessage($"La distance doit être supérieure ou égale à {DeliveryConstants.MinDistance} km")
            .LessThanOrEqualTo(DeliveryConstants.MaxDistance)
            .WithMessage($"La distance ne peut pas dépasser {DeliveryConstants.MaxDistance} km");

        RuleFor(x => x.OriginAddress)
            .NotNull()
            .WithMessage("L'adresse d'origine est obligatoire")
            .SetValidator(new CreateAddressDtoValidator("origine"));

        RuleFor(x => x.DestinationAddress)
            .NotNull()
            .WithMessage("L'adresse de destination est obligatoire")
            .SetValidator(new CreateAddressDtoValidator("destination"));

        RuleFor(x => x)
            .Must(x => !AddressesAreEqual(x.OriginAddress, x.DestinationAddress))
            .WithMessage("L'adresse d'origine et de destination ne peuvent pas être identiques")
            .When(x => x.OriginAddress != null && x.DestinationAddress != null);
    }

    private static bool AddressesAreEqual(CreateAddressDto? origin, CreateAddressDto? destination)
    {
        if (origin == null || destination == null) return false;
        
        return origin.Street == destination.Street &&
               origin.City == destination.City &&
               origin.PostalCode == destination.PostalCode &&
               origin.Country == destination.Country;
    }
}

/// <summary>
/// Validateur pour les DTOs d'adresse
/// </summary>
public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressDtoValidator(string addressType = "adresse")
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage($"La rue de l'{addressType} est obligatoire")
            .MaximumLength(DeliveryConstants.MaxAddressFieldLength)
            .WithMessage($"La rue de l'{addressType} ne peut pas dépasser {DeliveryConstants.MaxAddressFieldLength} caractères");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage($"La ville de l'{addressType} est obligatoire")
            .MaximumLength(DeliveryConstants.MaxAddressFieldLength)
            .WithMessage($"La ville de l'{addressType} ne peut pas dépasser {DeliveryConstants.MaxAddressFieldLength} caractères");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage($"Le code postal de l'{addressType} est obligatoire")
            .MaximumLength(DeliveryConstants.MaxPostalCodeLength)
            .WithMessage($"Le code postal de l'{addressType} ne peut pas dépasser {DeliveryConstants.MaxPostalCodeLength} caractères");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage($"Le pays de l'{addressType} est obligatoire")
            .MaximumLength(DeliveryConstants.MaxCountryLength)
            .WithMessage($"Le pays de l'{addressType} ne peut pas dépasser {DeliveryConstants.MaxCountryLength} caractères");
    }
}