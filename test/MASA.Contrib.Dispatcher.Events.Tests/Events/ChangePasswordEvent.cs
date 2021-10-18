namespace MASA.Contrib.Dispatcher.Events.Tests.Events;

public class ChangePasswordEvent : IEvent
{
    public string Account { get; set; }

    public string Content { get; set; }

    public Guid Id => Guid.NewGuid();

    public DateTime CreationTime => DateTime.UtcNow;
}