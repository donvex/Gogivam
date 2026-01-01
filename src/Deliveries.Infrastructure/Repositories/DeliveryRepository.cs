using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Deliveries.Domain.Entities;
using Deliveries.Infrastructure.Persistence;

namespace Deliveries.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les livraisons utilisant Entity Framework Core
/// </summary>
public class DeliveryRepository : IDeliveryRepository
{
    private readonly DeliveryDbContext _context;
    private readonly ILogger<DeliveryRepository> _logger;

    public DeliveryRepository(DeliveryDbContext context, ILogger<DeliveryRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting delivery by ID: {DeliveryId}", id);

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (delivery != null)
            {
                _logger.LogDebug("Found delivery with ID: {DeliveryId}", id);
            }
            else
            {
                _logger.LogDebug("Delivery not found with ID: {DeliveryId}", id);
            }

            return delivery;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery by ID: {DeliveryId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Delivery>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all deliveries");

            var deliveries = await _context.Deliveries
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} deliveries", deliveries.Count);

            return deliveries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all deliveries");
            throw;
        }
    }

    public async Task<Delivery> AddAsync(Delivery delivery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Adding new delivery for client: {ClientName}", delivery.ClientName);

            await _context.Deliveries.AddAsync(delivery, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully added delivery with ID: {DeliveryId}", delivery.Id);

            return delivery;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding delivery for client: {ClientName}", delivery.ClientName);
            throw;
        }
    }

    public async Task<Delivery> UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating delivery with ID: {DeliveryId}", delivery.Id);

            _context.Deliveries.Update(delivery);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated delivery with ID: {DeliveryId}", delivery.Id);

            return delivery;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating delivery with ID: {DeliveryId}", delivery.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting delivery with ID: {DeliveryId}", id);

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (delivery == null)
            {
                _logger.LogWarning("Delivery not found for deletion: {DeliveryId}", id);
                return false;
            }

            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted delivery with ID: {DeliveryId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting delivery with ID: {DeliveryId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if delivery exists with ID: {DeliveryId}", id);

            var exists = await _context.Deliveries
                .AnyAsync(d => d.Id == id, cancellationToken);

            _logger.LogDebug("Delivery exists check for ID {DeliveryId}: {Exists}", id, exists);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if delivery exists with ID: {DeliveryId}", id);
            throw;
        }
    }
}