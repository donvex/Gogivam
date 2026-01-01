using MediatR;
using Deliveries.Domain.DTOs;

namespace Deliveries.Application.Queries.Deliveries;

/// <summary>
/// Requête pour récupérer une livraison par son ID
/// </summary>
public record GetDeliveryByIdQuery : IRequest<DeliveryResponseDto?>
{
    public Guid Id { get; init; }

    public GetDeliveryByIdQuery(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Requête pour récupérer toutes les livraisons
/// </summary>
public record GetAllDeliveriesQuery : IRequest<IEnumerable<DeliveryResponseDto>>
{
}

/// <summary>
/// Requête pour récupérer les livraisons par statut
/// </summary>
public record GetDeliveriesByStatusQuery : IRequest<IEnumerable<DeliveryResponseDto>>
{
    public string Status { get; init; } = string.Empty;

    public GetDeliveriesByStatusQuery(string status)
    {
        Status = status;
    }
}