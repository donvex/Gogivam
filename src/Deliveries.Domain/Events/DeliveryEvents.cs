namespace Deliveries.Domain.Events;

/// <summary>
/// Interface de base pour les événements du domaine
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

/// <summary>
/// Classe de base pour les événements du domaine
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Événement levé quand une livraison est créée
/// </summary>
public class DeliveryCreatedEvent : DomainEvent
{
    public Guid DeliveryId { get; }
    public string ClientName { get; }
    public decimal TotalPrice { get; }

    public DeliveryCreatedEvent(Guid deliveryId, string clientName, decimal totalPrice)
    {
        DeliveryId = deliveryId;
        ClientName = clientName;
        TotalPrice = totalPrice;
    }
}

/// <summary>
/// Événement levé quand le statut d'une livraison change
/// </summary>
public class DeliveryStatusChangedEvent : DomainEvent
{
    public Guid DeliveryId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }

    public DeliveryStatusChangedEvent(Guid deliveryId, string oldStatus, string newStatus)
    {
        DeliveryId = deliveryId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}