namespace Deliveries.Domain.Exceptions;

/// <summary>
/// Exception de base pour le domaine des livraisons
/// </summary>
public abstract class DeliveryDomainException : Exception
{
    protected DeliveryDomainException(string message) : base(message) { }
    protected DeliveryDomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception levée quand une livraison n'est pas trouvée
/// </summary>
public class DeliveryNotFoundException : DeliveryDomainException
{
    public DeliveryNotFoundException(Guid deliveryId) 
        : base($"La livraison avec l'ID {deliveryId} n'a pas été trouvée.") { }
}

/// <summary>
/// Exception levée lors d'une validation métier échouée
/// </summary>
public class DeliveryValidationException : DeliveryDomainException
{
    public DeliveryValidationException(string message) : base(message) { }
}

/// <summary>
/// Exception levée lors d'un calcul de prix invalide
/// </summary>
public class PricingCalculationException : DeliveryDomainException
{
    public PricingCalculationException(string message) : base(message) { }
    public PricingCalculationException(string message, Exception innerException) : base(message, innerException) { }
}