namespace MASA.Contrib.Dispatcher.Events;

public record Event : IEvent
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public Event() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Event(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
