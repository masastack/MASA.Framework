namespace MASA.Contribs.DDD.Domain.Tests.Events;

public class PaymentFailedIntegrationDomainEvent : IntegrationDomainEvent
{
    public PaymentFailedIntegrationDomainEvent()
    {
        this.Topic = typeof(PaymentFailedIntegrationDomainEvent).Name;
    }

    public string OrderId { get; set; }

    public override string Topic { get; set; }

    public override string ToString()
    {
        return $"OrderId:{OrderId}, Topic:{Topic}, {base.ToString()}";
    }
}
