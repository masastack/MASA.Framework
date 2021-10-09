namespace MASA.Contribs.DDD.Domain.Tests.Events;

public class PaymentSucceededDomainEvent : DomainEvent
{
    public string OrderId { get; set; }

    public override string ToString()
    {
        return $"OrderId:{OrderId}, {base.ToString()}";
    }
}

