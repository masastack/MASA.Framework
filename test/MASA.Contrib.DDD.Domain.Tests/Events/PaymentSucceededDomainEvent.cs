namespace MASA.Contrib.DDD.Domain.Tests.Events;

public record PaymentSucceededDomainEvent : DomainEvent
{
    public string OrderId { get; set; }
}

