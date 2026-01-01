using Deliveries.Domain.DTOs;

namespace Deliveries.Application.Interfaces;

/// <summary>
/// Interface du service applicatif pour les livraisons
/// </summary>
public interface IDeliveryApplicationService
{
    /// <summary>
    /// Crée une nouvelle livraison
    /// </summary>
    Task<DeliveryResponseDto> CreateDeliveryAsync(CreateDeliveryDto createDeliveryDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une livraison par son ID
    /// </summary>
    Task<DeliveryResponseDto?> GetDeliveryByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère toutes les livraisons
    /// </summary>
    Task<IEnumerable<DeliveryResponseDto>> GetAllDeliveriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les livraisons par statut
    /// </summary>
    Task<IEnumerable<DeliveryResponseDto>> GetDeliveriesByStatusAsync(string status, CancellationToken cancellationToken = default);
}