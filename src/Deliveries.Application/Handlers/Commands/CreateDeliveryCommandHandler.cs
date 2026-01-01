using MediatR;
using Microsoft.Extensions.Logging;
using Deliveries.Application.Commands.Deliveries;
using Deliveries.Application.Mappers;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Factories;
using Deliveries.Domain.DTOs;

namespace Deliveries.Application.Handlers.Commands;

/// <summary>
/// Handler pour la commande de création de livraison
/// </summary>
public class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommand, DeliveryResponseDto>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly DeliveryFactory _deliveryFactory;
    private readonly ILogger<CreateDeliveryCommandHandler> _logger;

    public CreateDeliveryCommandHandler(
        IDeliveryRepository deliveryRepository,
        DeliveryFactory deliveryFactory,
        ILogger<CreateDeliveryCommandHandler> logger)
    {
        _deliveryRepository = deliveryRepository ?? throw new ArgumentNullException(nameof(deliveryRepository));
        _deliveryFactory = deliveryFactory ?? throw new ArgumentNullException(nameof(deliveryFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DeliveryResponseDto> Handle(CreateDeliveryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating delivery for client: {ClientName}", request.ClientName);

            // Mapper les DTOs vers les value objects du domaine
            var originAddress = DeliveryMapper.MapToAddress(request.OriginAddress);
            var destinationAddress = DeliveryMapper.MapToAddress(request.DestinationAddress);

            // Créer la livraison via la factory (avec validation automatique)
            var delivery = _deliveryFactory.CreateDelivery(
                request.ClientName,
                originAddress,
                destinationAddress,
                request.Weight,
                request.Distance);

            // Sauvegarder la livraison
            var savedDelivery = await _deliveryRepository.AddAsync(delivery, cancellationToken);

            _logger.LogInformation("Delivery created successfully with ID: {DeliveryId}", savedDelivery.Id);

            // Mapper vers le DTO de réponse
            return DeliveryMapper.MapToResponseDto(savedDelivery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating delivery for client: {ClientName}", request.ClientName);
            throw;
        }
    }
}