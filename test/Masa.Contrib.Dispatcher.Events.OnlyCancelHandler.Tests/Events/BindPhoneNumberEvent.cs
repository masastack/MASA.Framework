namespace Masa.Contrib.Dispatcher.Events.OnlyCancelHandler.Tests.Events;

public record BindPhoneNumberEvent : Event
{
    public string AccountId { get; set; }

    public string PhoneNumber { get; set; }
}
