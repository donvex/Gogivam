using Deliveries.Domain.Events;
using Deliveries.Domain.Exceptions;

namespace Deliveries.Domain.Entities;

/// <summary>
/// Classe de base pour les entités du domaine avec support des événements
/// </summary>
public abstract class EntityBase
{
    private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Agrégat racine pour la gestion des événements
/// </summary>
public abstract class AggregateRoot : EntityBase
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected AggregateRoot()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}