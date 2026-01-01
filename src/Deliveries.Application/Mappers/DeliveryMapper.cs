using Deliveries.Domain.Entities;
using Deliveries.Domain.DTOs;

namespace Deliveries.Application.Mappers;

/// <summary>
/// Mapper pour convertir entre les entités du domaine et les DTOs
/// </summary>
public static class DeliveryMapper
{
    /// <summary>
    /// Convertit un CreateAddressDto en Address (value object)
    /// </summary>
    public static Address MapToAddress(CreateAddressDto dto)
    {
        return new Address(dto.Street, dto.City, dto.PostalCode, dto.Country);
    }

    /// <summary>
    /// Convertit une Address en AddressResponseDto
    /// </summary>
    public static AddressResponseDto MapToAddressResponseDto(Address address)
    {
        return new AddressResponseDto
        {
            Street = address.Street,
            City = address.City,
            PostalCode = address.PostalCode,
            Country = address.Country,
            FullAddress = address.ToString()
        };
    }

    /// <summary>
    /// Convertit un Price en PriceResponseDto
    /// </summary>
    public static PriceResponseDto MapToPriceResponseDto(Price price)
    {
        return new PriceResponseDto
        {
            BasePrice = price.BasePrice,
            WeightPrice = price.WeightPrice,
            DistancePrice = price.DistancePrice,
            TotalPrice = price.TotalPrice,
            Currency = price.Currency,
            FormattedPrice = price.ToString()
        };
    }

    /// <summary>
    /// Convertit une Delivery en DeliveryResponseDto
    /// </summary>
    public static DeliveryResponseDto MapToResponseDto(Delivery delivery)
    {
        return new DeliveryResponseDto
        {
            Id = delivery.Id,
            ClientName = delivery.ClientName,
            OriginAddress = MapToAddressResponseDto(delivery.OriginAddress),
            DestinationAddress = MapToAddressResponseDto(delivery.DestinationAddress),
            Weight = delivery.Weight,
            Distance = delivery.Distance,
            Status = delivery.Status.ToString(),
            Price = delivery.CalculatedPrice != null ? MapToPriceResponseDto(delivery.CalculatedPrice) : null,
            CreatedAt = delivery.CreatedAt,
            UpdatedAt = delivery.UpdatedAt
        };
    }

    /// <summary>
    /// Convertit un CreateDeliveryCommand en paramètres pour la factory
    /// </summary>
    public static (string clientName, Address originAddress, Address destinationAddress, double weight, double distance) 
        MapFromCreateCommand(CreateDeliveryDto dto)
    {
        var originAddress = MapToAddress(dto.OriginAddress);
        var destinationAddress = MapToAddress(dto.DestinationAddress);
        
        return (dto.ClientName, originAddress, destinationAddress, dto.Weight, dto.Distance);
    }
}