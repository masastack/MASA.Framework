namespace MASA.Contribs.DDD.Domain.Events;

public abstract class IntegrationDomainEvent : DomainEvent, IIntegrationDomainEvent
{
    public abstract string Topic { get; set; }
}
