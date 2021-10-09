namespace MASA.Contrib.Dispatcher.Events;

public class Event : IEvent
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public Event() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Event(Guid id, DateTime creationTime)
    {
        Id = id;
        CreationTime = creationTime;
    }

    public override string ToString()
    {
        return $"Id:{Id}, CreationTime:{CreationTime}";
    }
}
