namespace Deliveries.Domain.Constants;

/// <summary>
/// Constantes pour les limites métier des livraisons
/// </summary>
public static class DeliveryConstants
{
    /// <summary>
    /// Poids maximum autorisé pour une livraison (en kg)
    /// </summary>
    public const double MaxWeight = 1000.0;

    /// <summary>
    /// Poids minimum autorisé pour une livraison (en kg)
    /// </summary>
    public const double MinWeight = 0.1;

    /// <summary>
    /// Distance maximum autorisée pour une livraison (en km)
    /// </summary>
    public const double MaxDistance = 10000.0;

    /// <summary>
    /// Distance minimum autorisée pour une livraison (en km)
    /// </summary>
    public const double MinDistance = 0.1;

    /// <summary>
    /// Longueur maximum du nom du client
    /// </summary>
    public const int MaxClientNameLength = 100;

    /// <summary>
    /// Longueur minimum du nom du client
    /// </summary>
    public const int MinClientNameLength = 2;

    /// <summary>
    /// Longueur maximum des champs d'adresse
    /// </summary>
    public const int MaxAddressFieldLength = 200;

    /// <summary>
    /// Longueur maximum du code postal
    /// </summary>
    public const int MaxPostalCodeLength = 20;

    /// <summary>
    /// Longueur maximum du pays
    /// </summary>
    public const int MaxCountryLength = 100;
}

/// <summary>
/// Constantes pour les tarifs par défaut
/// </summary>
public static class PricingConstants
{
    /// <summary>
    /// Prix de base par défaut (en euros)
    /// </summary>
    public const decimal DefaultBasePrice = 5.0m;

    /// <summary>
    /// Prix par kilomètre par défaut (en euros)
    /// </summary>
    public const decimal DefaultPricePerKm = 0.5m;

    /// <summary>
    /// Prix par kilogramme par défaut (en euros)
    /// </summary>
    public const decimal DefaultPricePerKg = 1.0m;

    /// <summary>
    /// Devise par défaut
    /// </summary>
    public const string DefaultCurrency = "EUR";

    /// <summary>
    /// Prix minimum pour une livraison (en euros)
    /// </summary>
    public const decimal MinimumPrice = 1.0m;
}