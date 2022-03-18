namespace Masa.Contrib.Dispatcher.Events.HandlerOrder.Tests.Events;

public record CalculateEvent : Event
{
    public int ParameterA { get; set; }

    public int ParameterB { get; set; }

    public int Result { get; set; }
}
