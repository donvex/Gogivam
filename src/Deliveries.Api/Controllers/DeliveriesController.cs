using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Deliveries.Application.Interfaces;
using Deliveries.Domain.DTOs;
using FluentValidation;
using System.Net;

namespace Deliveries.Api.Controllers;

/// <summary>
/// Contrôleur pour la gestion des livraisons
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryApplicationService _deliveryService;
    private readonly ILogger<DeliveriesController> _logger;

    public DeliveriesController(
        IDeliveryApplicationService deliveryService,
        ILogger<DeliveriesController> logger)
    {
        _deliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crée une nouvelle livraison
    /// </summary>
    /// <param name="createDeliveryDto">Données de la livraison à créer</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>La livraison créée avec son prix calculé</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Créer une nouvelle livraison",
        Description = "Crée une nouvelle livraison avec calcul automatique du prix basé sur la règle : prix_de_base + distance + poids",
        OperationId = "CreateDelivery"
    )]
    [SwaggerResponse((int)HttpStatusCode.Created, "Livraison créée avec succès", typeof(DeliveryResponseDto))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Données de la requête invalides", typeof(ValidationProblemDetails))]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erreur interne du serveur")]
    [ProducesResponseType(typeof(DeliveryResponseDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateDelivery(
        [FromBody, SwaggerParameter("Données de la livraison", Required = true)] CreateDeliveryDto createDeliveryDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Création d'une nouvelle livraison pour le client: {ClientName}", createDeliveryDto.ClientName);

            // Validation manuelle si nécessaire (FluentValidation est configuré automatiquement)
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation échouée pour la création de livraison: {ValidationErrors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            var delivery = await _deliveryService.CreateDeliveryAsync(createDeliveryDto, cancellationToken);

            _logger.LogInformation("Livraison créée avec succès avec l'ID: {DeliveryId}", delivery.Id);

            return CreatedAtAction(
                nameof(GetDeliveryById), 
                new { id = delivery.Id }, 
                delivery);
        }
        catch (ValidationException validationEx)
        {
            _logger.LogWarning("Erreur de validation lors de la création de livraison: {ValidationError}", validationEx.Message);
            
            var problemDetails = new ValidationProblemDetails();
            foreach (var error in validationEx.Errors)
            {
                if (!problemDetails.Errors.ContainsKey(error.PropertyName))
                {
                    problemDetails.Errors[error.PropertyName] = new string[] { };
                }
                problemDetails.Errors[error.PropertyName] = problemDetails.Errors[error.PropertyName]
                    .Concat(new[] { error.ErrorMessage }).ToArray();
            }
            
            return BadRequest(problemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la livraison pour le client: {ClientName}", createDeliveryDto.ClientName);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Une erreur interne s'est produite");
        }
    }

    /// <summary>
    /// Récupère une livraison par son ID
    /// </summary>
    /// <param name="id">ID de la livraison</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>La livraison correspondante ou NotFound si elle n'existe pas</returns>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Récupérer une livraison par ID",
        Description = "Récupère les détails complets d'une livraison en utilisant son identifiant unique",
        OperationId = "GetDeliveryById"
    )]
    [SwaggerResponse((int)HttpStatusCode.OK, "Livraison trouvée", typeof(DeliveryResponseDto))]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Livraison introuvable")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "ID invalide")]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erreur interne du serveur")]
    [ProducesResponseType(typeof(DeliveryResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetDeliveryById(
        [FromRoute, SwaggerParameter("ID unique de la livraison", Required = true)] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Récupération de la livraison avec l'ID: {DeliveryId}", id);

            if (id == Guid.Empty)
            {
                _logger.LogWarning("ID de livraison vide fourni");
                return BadRequest("L'ID de la livraison ne peut pas être vide");
            }

            var delivery = await _deliveryService.GetDeliveryByIdAsync(id, cancellationToken);

            if (delivery == null)
            {
                _logger.LogWarning("Livraison introuvable avec l'ID: {DeliveryId}", id);
                return NotFound($"Aucune livraison trouvée avec l'ID: {id}");
            }

            _logger.LogInformation("Livraison récupérée avec succès: {DeliveryId}", id);
            return Ok(delivery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la livraison avec l'ID: {DeliveryId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Une erreur interne s'est produite");
        }
    }

    /// <summary>
    /// Récupère toutes les livraisons (endpoint bonus)
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste de toutes les livraisons</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Récupérer toutes les livraisons",
        Description = "Récupère la liste de toutes les livraisons du système",
        OperationId = "GetAllDeliveries"
    )]
    [SwaggerResponse((int)HttpStatusCode.OK, "Liste des livraisons", typeof(IEnumerable<DeliveryResponseDto>))]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Erreur interne du serveur")]
    [ProducesResponseType(typeof(IEnumerable<DeliveryResponseDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllDeliveries(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Récupération de toutes les livraisons");

            var deliveries = await _deliveryService.GetAllDeliveriesAsync(cancellationToken);

            _logger.LogInformation("Récupération réussie de {Count} livraisons", deliveries.Count());
            return Ok(deliveries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de toutes les livraisons");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Une erreur interne s'est produite");
        }
    }
}