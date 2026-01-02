using System.ComponentModel.DataAnnotations;
using Deliveries.Domain.Events;
using Deliveries.Domain.Constants;

namespace Deliveries.Domain.Entities;

/// <summary>
/// Représente une livraison dans le système
/// </summary>
public class Delivery : AggregateRoot
{
    public string? ClientName { get; private set; }
    public Address? OriginAddress { get; private set; }
    public Address? DestinationAddress { get; private set; }
    public double Weight { get; private set; } // en kg
    public double Distance { get; private set; } // en km
    public DeliveryStatus Status { get; private set; }
    public Price? CalculatedPrice { get; private set; }

    // Constructeur privé pour EF Core
    private Delivery() : base() { }

    /// <summary>
    /// Constructeur pour créer une nouvelle livraison
    /// </summary>
    public Delivery(
        string clientName,
        Address originAddress,
        Address destinationAddress,
        double weight,
        double distance) : base()
    {
        ClientName = clientName ?? throw new ArgumentNullException(nameof(clientName));
        OriginAddress = originAddress ?? throw new ArgumentNullException(nameof(originAddress));
        DestinationAddress = destinationAddress ?? throw new ArgumentNullException(nameof(destinationAddress));
        Weight = weight > 0 ? weight : throw new ArgumentException("Le poids doit être positif", nameof(weight));
        Distance = distance > 0 ? distance : throw new ArgumentException("La distance doit être positive", nameof(distance));
        Status = DeliveryStatus.Pending;
        
        // Ajouter événement de création
        AddDomainEvent(new DeliveryCreatedEvent(Id, clientName, 0)); // Prix sera calculé séparément
    }

    /// <summary>
    /// Calcule le prix de la livraison
    /// </summary>
    public void CalculatePrice(IPricingService pricingService)
    {
        CalculatedPrice = pricingService.CalculatePrice(Weight, Distance);
        UpdateTimestamp();
    }

    /// <summary>
    /// Met à jour le statut de la livraison
    /// </summary>
    public void UpdateStatus(DeliveryStatus newStatus)
    {
        var oldStatus = Status.ToString();
        Status = newStatus;
        UpdateTimestamp();
        
        // Ajouter événement de changement de statut
        AddDomainEvent(new DeliveryStatusChangedEvent(Id, oldStatus, newStatus.ToString()));
    }
}

/// <summary>
/// Value Object représentant une adresse
/// </summary>
public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    public Address(string street, string city, string postalCode, string country)
    {
        Street = !string.IsNullOrWhiteSpace(street) ? street : throw new ArgumentException("La rue est obligatoire", nameof(street));
        City = !string.IsNullOrWhiteSpace(city) ? city : throw new ArgumentException("La ville est obligatoire", nameof(city));
        PostalCode = !string.IsNullOrWhiteSpace(postalCode) ? postalCode : throw new ArgumentException("Le code postal est obligatoire", nameof(postalCode));
        Country = !string.IsNullOrWhiteSpace(country) ? country : throw new ArgumentException("Le pays est obligatoire", nameof(country));
    }

    public override string ToString()
    {
        return $"{Street}, {City} {PostalCode}, {Country}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Address address &&
               Street == address.Street &&
               City == address.City &&
               PostalCode == address.PostalCode &&
               Country == address.Country;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, PostalCode, Country);
    }
}

/// <summary>
/// Value Object représentant un prix
/// </summary>
public class Price
{
    public decimal BasePrice { get; private set; }
    public decimal WeightPrice { get; private set; }
    public decimal DistancePrice { get; private set; }
    public decimal TotalPrice => BasePrice + WeightPrice + DistancePrice;
    public string Currency { get; private set; }

    public Price(decimal basePrice, decimal weightPrice, decimal distancePrice, string currency = "EUR")
    {
        BasePrice = basePrice >= 0 ? basePrice : throw new ArgumentException("Le prix de base doit être positif ou nul", nameof(basePrice));
        WeightPrice = weightPrice >= 0 ? weightPrice : throw new ArgumentException("Le prix du poids doit être positif ou nul", nameof(weightPrice));
        DistancePrice = distancePrice >= 0 ? distancePrice : throw new ArgumentException("Le prix de la distance doit être positif ou nul", nameof(distancePrice));
        Currency = !string.IsNullOrWhiteSpace(currency) ? currency : throw new ArgumentException("La devise est obligatoire", nameof(currency));
    }

    public override string ToString()
    {
        return $"{TotalPrice:F2} {Currency}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Price price &&
               BasePrice == price.BasePrice &&
               WeightPrice == price.WeightPrice &&
               DistancePrice == price.DistancePrice &&
               Currency == price.Currency;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BasePrice, WeightPrice, DistancePrice, Currency);
    }
}

/// <summary>
/// Énumération des statuts de livraison
/// </summary>
public enum DeliveryStatus
{
    [Display(Name = "En attente")]
    Pending = 0,
    
    [Display(Name = "Confirmée")]
    Confirmed = 1,
    
    [Display(Name = "En transit")]
    InTransit = 2,
    
    [Display(Name = "Livrée")]
    Delivered = 3,
    
    [Display(Name = "Annulée")]
    Cancelled = 4,
    
    [Display(Name = "Échec de livraison")]
    Failed = 5
}

/// <summary>
/// Interface pour le service de tarification
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Calcule le prix d'une livraison basé sur le poids et la distance
    /// </summary>
    /// <param name="weight">Poids en kg</param>
    /// <param name="distance">Distance en km</param>
    /// <returns>Prix calculé</returns>
    Price CalculatePrice(double weight, double distance);
}

/// <summary>
/// Interface de repository pour les livraisons
/// </summary>
public interface IDeliveryRepository
{
    Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Delivery>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Delivery> AddAsync(Delivery delivery, CancellationToken cancellationToken = default);
    Task<Delivery> UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}