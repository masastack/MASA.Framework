namespace MASA.Contrib.Dispatcher.InMemory.Tests.Events;

public class ShipOrderEvent : Event
{
    public string OrderId { get; set; }

    public string OrderState { get; set; }

    public string Message { get; set; }
}