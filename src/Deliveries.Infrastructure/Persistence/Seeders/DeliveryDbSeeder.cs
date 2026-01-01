using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Factories;
using Deliveries.Infrastructure.Persistence;

namespace Deliveries.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder pour initialiser la base de données avec des données de test
/// </summary>
public class DeliveryDbSeeder
{
    private readonly DeliveryDbContext _context;
    private readonly DeliveryFactory _deliveryFactory;
    private readonly ILogger<DeliveryDbSeeder> _logger;

    public DeliveryDbSeeder(
        DeliveryDbContext context,
        DeliveryFactory deliveryFactory,
        ILogger<DeliveryDbSeeder> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _deliveryFactory = deliveryFactory ?? throw new ArgumentNullException(nameof(deliveryFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Initialise la base de données avec des données de test
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Vérifier si des données existent déjà
            if (await _context.Deliveries.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            // Créer des livraisons de test
            var deliveries = CreateSampleDeliveries();

            // Ajouter les livraisons à la base
            await _context.Deliveries.AddRangeAsync(deliveries);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} deliveries", deliveries.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            throw;
        }
    }

    private List<Delivery> CreateSampleDeliveries()
    {
        var deliveries = new List<Delivery>();

        // Livraison 1 : Paris -> Lyon
        var delivery1 = _deliveryFactory.CreateDelivery(
            "Jean Dupont",
            new Address("123 Avenue des Champs-Élysées", "Paris", "75008", "France"),
            new Address("45 Place Bellecour", "Lyon", "69002", "France"),
            5.0,
            465.0
        );
        deliveries.Add(delivery1);

        // Livraison 2 : Marseille -> Nice
        var delivery2 = _deliveryFactory.CreateDelivery(
            "Marie Martin",
            new Address("12 Rue de la République", "Marseille", "13001", "France"),
            new Address("78 Promenade des Anglais", "Nice", "06000", "France"),
            2.5,
            200.0
        );
        deliveries.Add(delivery2);

        // Livraison 3 : Toulouse -> Bordeaux
        var delivery3 = _deliveryFactory.CreateDelivery(
            "Pierre Durand",
            new Address("34 Place du Capitole", "Toulouse", "31000", "France"),
            new Address("56 Cours de l'Intendance", "Bordeaux", "33000", "France"),
            10.0,
            245.0
        );
        deliveries.Add(delivery3);

        // Livraison 4 : Lille -> Strasbourg
        var delivery4 = _deliveryFactory.CreateDelivery(
            "Sophie Petit",
            new Address("89 Grand Place", "Lille", "59000", "France"),
            new Address("23 Place Kléber", "Strasbourg", "67000", "France"),
            7.5,
            520.0
        );
        deliveries.Add(delivery4);

        // Livraison 5 : Nantes -> Rennes
        var delivery5 = _deliveryFactory.CreateDelivery(
            "Michel Blanc",
            new Address("67 Cours des 50 Otages", "Nantes", "44000", "France"),
            new Address("12 Place des Lices", "Rennes", "35000", "France"),
            3.0,
            110.0
        );
        deliveries.Add(delivery5);

        return deliveries;
    }
}