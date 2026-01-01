using MediatR;
using Microsoft.Extensions.Logging;
using Deliveries.Application.Queries.Deliveries;
using Deliveries.Application.Mappers;
using Deliveries.Domain.Entities;
using Deliveries.Domain.DTOs;

namespace Deliveries.Application.Handlers.Queries;

/// <summary>
/// Handler pour la requête de récupération d'une livraison par ID
/// </summary>
public class GetDeliveryByIdQueryHandler : IRequestHandler<GetDeliveryByIdQuery, DeliveryResponseDto?>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<GetDeliveryByIdQueryHandler> _logger;

    public GetDeliveryByIdQueryHandler(
        IDeliveryRepository deliveryRepository,
        ILogger<GetDeliveryByIdQueryHandler> logger)
    {
        _deliveryRepository = deliveryRepository ?? throw new ArgumentNullException(nameof(deliveryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DeliveryResponseDto?> Handle(GetDeliveryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving delivery with ID: {DeliveryId}", request.Id);

            var delivery = await _deliveryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (delivery == null)
            {
                _logger.LogWarning("Delivery not found with ID: {DeliveryId}", request.Id);
                return null;
            }

            _logger.LogInformation("Delivery found with ID: {DeliveryId}", request.Id);
            return DeliveryMapper.MapToResponseDto(delivery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving delivery with ID: {DeliveryId}", request.Id);
            throw;
        }
    }
}

/// <summary>
/// Handler pour la requête de récupération de toutes les livraisons
/// </summary>
public class GetAllDeliveriesQueryHandler : IRequestHandler<GetAllDeliveriesQuery, IEnumerable<DeliveryResponseDto>>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<GetAllDeliveriesQueryHandler> _logger;

    public GetAllDeliveriesQueryHandler(
        IDeliveryRepository deliveryRepository,
        ILogger<GetAllDeliveriesQueryHandler> logger)
    {
        _deliveryRepository = deliveryRepository ?? throw new ArgumentNullException(nameof(deliveryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<DeliveryResponseDto>> Handle(GetAllDeliveriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving all deliveries");

            var deliveries = await _deliveryRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Found {Count} deliveries", deliveries.Count());

            return deliveries.Select(DeliveryMapper.MapToResponseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all deliveries");
            throw;
        }
    }
}

/// <summary>
/// Handler pour la requête de récupération des livraisons par statut
/// </summary>
public class GetDeliveriesByStatusQueryHandler : IRequestHandler<GetDeliveriesByStatusQuery, IEnumerable<DeliveryResponseDto>>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<GetDeliveriesByStatusQueryHandler> _logger;

    public GetDeliveriesByStatusQueryHandler(
        IDeliveryRepository deliveryRepository,
        ILogger<GetDeliveriesByStatusQueryHandler> logger)
    {
        _deliveryRepository = deliveryRepository ?? throw new ArgumentNullException(nameof(deliveryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<DeliveryResponseDto>> Handle(GetDeliveriesByStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving deliveries with status: {Status}", request.Status);

            // Convertir le string en enum
            if (!Enum.TryParse<DeliveryStatus>(request.Status, true, out var status))
            {
                _logger.LogWarning("Invalid delivery status: {Status}", request.Status);
                return Enumerable.Empty<DeliveryResponseDto>();
            }

            var allDeliveries = await _deliveryRepository.GetAllAsync(cancellationToken);
            var filteredDeliveries = allDeliveries.Where(d => d.Status == status);

            _logger.LogInformation("Found {Count} deliveries with status {Status}", filteredDeliveries.Count(), status);

            return filteredDeliveries.Select(DeliveryMapper.MapToResponseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deliveries with status: {Status}", request.Status);
            throw;
        }
    }
}