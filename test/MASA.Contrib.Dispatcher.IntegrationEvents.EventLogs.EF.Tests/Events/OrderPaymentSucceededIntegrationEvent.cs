namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests.Events;

internal class OrderPaymentSucceededIntegrationEvent : IntegrationEvent
{
    public string OrderId { get; set; }

    public long PaymentTime { get; set; }

    public override string Topic { get; set; } = nameof(OrderPaymentSucceededIntegrationEvent);

    public override string ToString()
    {
        return $"OrderId:{OrderId}, PaymentTime:{PaymentTime}, Topic：{Topic}, {base.ToString()}";
    }
}
