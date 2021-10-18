namespace MASA.Contrib.Dispatcher.Events.OnlyCancelHandler.Tests.Events;

public record BindMobileEvent : Event
{
    public string AccountId { get; set; }

    public string Mobile { get; set; }
}
