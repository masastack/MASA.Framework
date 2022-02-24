namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record OrderPaymentSucceededEvent : Event
{
    public string OrderId { get; set; }

    public long Timespan { get; set; }
}
