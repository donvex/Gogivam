using Deliveries.Domain.Entities;
using Deliveries.Domain.Validators;

namespace Deliveries.Domain.Factories;

/// <summary>
/// Factory pour créer des livraisons avec validation
/// </summary>
public class DeliveryFactory
{
    private readonly IPricingService _pricingService;

    public DeliveryFactory(IPricingService pricingService)
    {
        _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
    }

    /// <summary>
    /// Crée une nouvelle livraison avec validation complète
    /// </summary>
    public Delivery CreateDelivery(
        string clientName,
        Address originAddress,
        Address destinationAddress,
        double weight,
        double distance)
    {
        // Validation avant création
        DeliveryValidator.ValidateForCreation(clientName, originAddress, destinationAddress, weight, distance);
        DeliveryValidator.ValidateAddress(originAddress, "l'adresse d'origine");
        DeliveryValidator.ValidateAddress(destinationAddress, "l'adresse de destination");

        // Création de la livraison
        var delivery = new Delivery(clientName, originAddress, destinationAddress, weight, distance);

        // Calcul automatique du prix
        delivery.CalculatePrice(_pricingService);

        return delivery;
    }

    /// <summary>
    /// Crée une nouvelle livraison avec un prix personnalisé (pour les cas spéciaux)
    /// </summary>
    public Delivery CreateDeliveryWithCustomPrice(
        string clientName,
        Address originAddress,
        Address destinationAddress,
        double weight,
        double distance,
        Price customPrice)
    {
        // Validation avant création
        DeliveryValidator.ValidateForCreation(clientName, originAddress, destinationAddress, weight, distance);
        DeliveryValidator.ValidateAddress(originAddress, "l'adresse d'origine");
        DeliveryValidator.ValidateAddress(destinationAddress, "l'adresse de destination");

        // Création de la livraison
        var delivery = new Delivery(clientName, originAddress, destinationAddress, weight, distance);

        // Attribution du prix personnalisé
        // Note: Pour cela, il faudrait ajouter une méthode SetCustomPrice à l'entité Delivery
        // ou modifier l'architecture pour permettre les prix personnalisés

        return delivery;
    }
}