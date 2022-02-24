namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests.Events;

internal record OrderPaymentSucceededIntegrationEvent : IntegrationEvent
{
    public string OrderId { get; set; }

    public long PaymentTime { get; set; }

    public override string Topic { get; set; } = nameof(OrderPaymentSucceededIntegrationEvent);
}
