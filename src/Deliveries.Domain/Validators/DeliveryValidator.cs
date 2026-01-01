using Deliveries.Domain.Entities;
using Deliveries.Domain.Exceptions;
using Deliveries.Domain.Constants;

namespace Deliveries.Domain.Validators;

/// <summary>
/// Validateur pour les entités de livraison
/// </summary>
public static class DeliveryValidator
{
    /// <summary>
    /// Valide une livraison avant sa création
    /// </summary>
    public static void ValidateForCreation(
        string clientName,
        Address originAddress,
        Address destinationAddress,
        double weight,
        double distance)
    {
        var errors = new List<string>();

        // Validation du nom du client
        if (string.IsNullOrWhiteSpace(clientName))
        {
            errors.Add("Le nom du client est obligatoire");
        }
        else if (clientName.Length < DeliveryConstants.MinClientNameLength)
        {
            errors.Add($"Le nom du client doit contenir au moins {DeliveryConstants.MinClientNameLength} caractères");
        }
        else if (clientName.Length > DeliveryConstants.MaxClientNameLength)
        {
            errors.Add($"Le nom du client ne peut pas dépasser {DeliveryConstants.MaxClientNameLength} caractères");
        }

        // Validation des adresses
        if (originAddress == null)
        {
            errors.Add("L'adresse d'origine est obligatoire");
        }

        if (destinationAddress == null)
        {
            errors.Add("L'adresse de destination est obligatoire");
        }

        if (originAddress != null && destinationAddress != null && originAddress.Equals(destinationAddress))
        {
            errors.Add("L'adresse d'origine et de destination ne peuvent pas être identiques");
        }

        // Validation du poids
        if (weight < DeliveryConstants.MinWeight)
        {
            errors.Add($"Le poids doit être supérieur à {DeliveryConstants.MinWeight} kg");
        }
        else if (weight > DeliveryConstants.MaxWeight)
        {
            errors.Add($"Le poids ne peut pas dépasser {DeliveryConstants.MaxWeight} kg");
        }

        // Validation de la distance
        if (distance < DeliveryConstants.MinDistance)
        {
            errors.Add($"La distance doit être supérieure à {DeliveryConstants.MinDistance} km");
        }
        else if (distance > DeliveryConstants.MaxDistance)
        {
            errors.Add($"La distance ne peut pas dépasser {DeliveryConstants.MaxDistance} km");
        }

        if (errors.Any())
        {
            throw new DeliveryValidationException($"Erreurs de validation : {string.Join(", ", errors)}");
        }
    }

    /// <summary>
    /// Valide une adresse
    /// </summary>
    public static void ValidateAddress(Address address, string fieldName)
    {
        var errors = new List<string>();

        if (address == null)
        {
            errors.Add($"{fieldName} est obligatoire");
            return;
        }

        // Validation des champs de l'adresse
        if (string.IsNullOrWhiteSpace(address.Street))
        {
            errors.Add($"La rue de {fieldName.ToLower()} est obligatoire");
        }
        else if (address.Street.Length > DeliveryConstants.MaxAddressFieldLength)
        {
            errors.Add($"La rue de {fieldName.ToLower()} ne peut pas dépasser {DeliveryConstants.MaxAddressFieldLength} caractères");
        }

        if (string.IsNullOrWhiteSpace(address.City))
        {
            errors.Add($"La ville de {fieldName.ToLower()} est obligatoire");
        }
        else if (address.City.Length > DeliveryConstants.MaxAddressFieldLength)
        {
            errors.Add($"La ville de {fieldName.ToLower()} ne peut pas dépasser {DeliveryConstants.MaxAddressFieldLength} caractères");
        }

        if (string.IsNullOrWhiteSpace(address.PostalCode))
        {
            errors.Add($"Le code postal de {fieldName.ToLower()} est obligatoire");
        }
        else if (address.PostalCode.Length > DeliveryConstants.MaxPostalCodeLength)
        {
            errors.Add($"Le code postal de {fieldName.ToLower()} ne peut pas dépasser {DeliveryConstants.MaxPostalCodeLength} caractères");
        }

        if (string.IsNullOrWhiteSpace(address.Country))
        {
            errors.Add($"Le pays de {fieldName.ToLower()} est obligatoire");
        }
        else if (address.Country.Length > DeliveryConstants.MaxCountryLength)
        {
            errors.Add($"Le pays de {fieldName.ToLower()} ne peut pas dépasser {DeliveryConstants.MaxCountryLength} caractères");
        }

        if (errors.Any())
        {
            throw new DeliveryValidationException($"Erreurs de validation pour {fieldName} : {string.Join(", ", errors)}");
        }
    }
}