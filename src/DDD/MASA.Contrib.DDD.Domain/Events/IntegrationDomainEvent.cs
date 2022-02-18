namespace MASA.Contrib.DDD.Domain.Events;

public abstract record IntegrationDomainEvent(Guid Id, DateTime CreationTime) : DomainEvent(Id, CreationTime), IIntegrationDomainEvent
{
    [JsonIgnore]
    public abstract string Topic { get; set; }

    public IntegrationDomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
