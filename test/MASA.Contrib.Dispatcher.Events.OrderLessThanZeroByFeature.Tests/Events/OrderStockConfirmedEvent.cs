namespace MASA.Contrib.Dispatcher.Events.OrderLessThanZeroByFeature.Tests.Events;

public record OrderStockConfirmedEvent : Event
{
    public string OrderId { get; set; }
}
