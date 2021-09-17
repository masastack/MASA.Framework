namespace MASA.Contrib.Dispatcher.InMemory.Tests.Events;

public class EditUserEvent : Event
{
    public string UserId { get; set; }

    public string UserName { get; set; }
}