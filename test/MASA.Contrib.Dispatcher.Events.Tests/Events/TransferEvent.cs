namespace MASA.Contrib.Dispatcher.Events.Tests.Events;

public record TransferEvent : Event
{
    public string Account { get; set; }

    public string OptAccount { get; set; }

    public decimal Price { get; set; }
}
