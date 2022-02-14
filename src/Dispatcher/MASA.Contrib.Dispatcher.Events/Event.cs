namespace MASA.Contrib.Dispatcher.Events;

public record Event(Guid Id, DateTime CreationTime) : IEvent
{
    public Event() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
