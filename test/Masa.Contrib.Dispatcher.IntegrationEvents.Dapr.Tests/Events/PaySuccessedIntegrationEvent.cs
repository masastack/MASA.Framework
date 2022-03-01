namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Events;

public record PaySuccessedIntegrationEvent() : IntegrationEvent()
{
    public string OrderNo { get; set; }

    public override string Topic { get; set; } = nameof(PaySuccessedIntegrationEvent);

    public PaySuccessedIntegrationEvent(string orderNo) : this()
    {
        OrderNo = orderNo;
    }
}
