namespace MASA.Contrib.DDD.Domain.Events;

public abstract record IntegrationDomainEvent : DomainEvent, IIntegrationDomainEvent
{
    public abstract string Topic { get; set; }

    public IntegrationDomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public IntegrationDomainEvent(Guid id, DateTime creationTime) : base(id, creationTime)
    {
    }
}
