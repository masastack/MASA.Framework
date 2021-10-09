namespace MASA.Contrib.Dispatcher.InMemory.OrderLessThanZeroByFeature.Tests.Events;

public class OrderStockConfirmedEvent : Event
{
    public string OrderId { get; set; }
}