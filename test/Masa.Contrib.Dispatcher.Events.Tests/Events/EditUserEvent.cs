namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record EditUserEvent : Event
{
    public string UserId { get; set; }

    public string UserName { get; set; }
}
