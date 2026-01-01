using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Deliveries.Application.Interfaces;
using Deliveries.Application.Commands.Deliveries;
using Deliveries.Application.Queries.Deliveries;
using Deliveries.Domain.DTOs;

namespace Deliveries.Application.Services;

/// <summary>
/// Service applicatif pour les livraisons utilisant MediatR
/// </summary>
public class DeliveryApplicationService : IDeliveryApplicationService
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDeliveryCommand> _createDeliveryValidator;
    private readonly ILogger<DeliveryApplicationService> _logger;

    public DeliveryApplicationService(
        IMediator mediator,
        IValidator<CreateDeliveryCommand> createDeliveryValidator,
        ILogger<DeliveryApplicationService> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _createDeliveryValidator = createDeliveryValidator ?? throw new ArgumentNullException(nameof(createDeliveryValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DeliveryResponseDto> CreateDeliveryAsync(CreateDeliveryDto createDeliveryDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating delivery for client: {ClientName}", createDeliveryDto.ClientName);

        // Créer la commande
        var command = new CreateDeliveryCommand
        {
            ClientName = createDeliveryDto.ClientName,
            OriginAddress = createDeliveryDto.OriginAddress,
            DestinationAddress = createDeliveryDto.DestinationAddress,
            Weight = createDeliveryDto.Weight,
            Distance = createDeliveryDto.Distance
        };

        // Valider la commande
        var validationResult = await _createDeliveryValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed for delivery creation: {Errors}", errors);
            throw new ValidationException(validationResult.Errors);
        }

        // Exécuter la commande
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Delivery created successfully with ID: {DeliveryId}", result.Id);
        return result;
    }

    public async Task<DeliveryResponseDto?> GetDeliveryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving delivery with ID: {DeliveryId}", id);

        var query = new GetDeliveryByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Delivery not found with ID: {DeliveryId}", id);
        }
        else
        {
            _logger.LogInformation("Delivery retrieved successfully with ID: {DeliveryId}", id);
        }

        return result;
    }

    public async Task<IEnumerable<DeliveryResponseDto>> GetAllDeliveriesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all deliveries");

        var query = new GetAllDeliveriesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        _logger.LogInformation("Retrieved {Count} deliveries", result.Count());
        return result;
    }

    public async Task<IEnumerable<DeliveryResponseDto>> GetDeliveriesByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving deliveries with status: {Status}", status);

        var query = new GetDeliveriesByStatusQuery(status);
        var result = await _mediator.Send(query, cancellationToken);

        _logger.LogInformation("Retrieved {Count} deliveries with status: {Status}", result.Count(), status);
        return result;
    }
}