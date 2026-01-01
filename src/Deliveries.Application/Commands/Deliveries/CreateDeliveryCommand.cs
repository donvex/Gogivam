using MediatR;
using Deliveries.Domain.DTOs;

namespace Deliveries.Application.Commands.Deliveries;

/// <summary>
/// Commande pour créer une nouvelle livraison
/// </summary>
public record CreateDeliveryCommand : IRequest<DeliveryResponseDto>
{
    public string ClientName { get; init; } = string.Empty;
    public CreateAddressDto OriginAddress { get; init; } = new();
    public CreateAddressDto DestinationAddress { get; init; } = new();
    public double Weight { get; init; }
    public double Distance { get; init; }
}

/// <summary>
/// Résultat de la création d'une livraison
/// </summary>
public record CreateDeliveryResult
{
    public bool IsSuccess { get; init; }
    public DeliveryResponseDto? Delivery { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string> ValidationErrors { get; init; } = new();

    public static CreateDeliveryResult Success(DeliveryResponseDto delivery)
        => new() { IsSuccess = true, Delivery = delivery };

    public static CreateDeliveryResult Failure(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage };

    public static CreateDeliveryResult ValidationFailure(List<string> validationErrors)
        => new() { IsSuccess = false, ValidationErrors = validationErrors };
}