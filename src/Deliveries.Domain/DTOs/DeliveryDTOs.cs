namespace Deliveries.Domain.DTOs;

/// <summary>
/// DTO pour la création d'une livraison
/// </summary>
public class CreateDeliveryDto
{
    public string ClientName { get; set; } = string.Empty;
    public CreateAddressDto OriginAddress { get; set; } = new();
    public CreateAddressDto DestinationAddress { get; set; } = new();
    public double Weight { get; set; }
    public double Distance { get; set; }
}

/// <summary>
/// DTO pour la création d'une adresse
/// </summary>
public class CreateAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

/// <summary>
/// DTO pour la réponse d'une livraison créée
/// </summary>
public class DeliveryResponseDto
{
    public Guid Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public AddressResponseDto OriginAddress { get; set; } = new();
    public AddressResponseDto DestinationAddress { get; set; } = new();
    public double Weight { get; set; }
    public double Distance { get; set; }
    public string Status { get; set; } = string.Empty;
    public PriceResponseDto? Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO pour la réponse d'une adresse
/// </summary>
public class AddressResponseDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
}

/// <summary>
/// DTO pour la réponse d'un prix
/// </summary>
public class PriceResponseDto
{
    public decimal BasePrice { get; set; }
    public decimal WeightPrice { get; set; }
    public decimal DistancePrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string FormattedPrice { get; set; } = string.Empty;
}