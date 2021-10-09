namespace MASA.Contrib.Dispatcher.InMemory.OnlyCancelHandler.Tests.Events;

public class BindMobileEvent : Event
{
    public string AccountId { get; set; }

    public string Mobile { get; set; }
}
