namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record ShipOrderEvent : Event
{
    public string OrderId { get; set; }

    public string OrderState { get; set; }

    public string Message { get; set; }
}
