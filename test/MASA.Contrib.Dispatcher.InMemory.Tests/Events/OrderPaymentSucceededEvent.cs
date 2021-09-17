namespace MASA.Contrib.Dispatcher.InMemory.Tests.Events;

public class OrderPaymentSucceededEvent : Event
{
    public string OrderId { get; set; }

    public long Timespan { get; set; }
}