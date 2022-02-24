namespace Masa.Contrib.Dispatcher.Events;

public record Event(Guid Id, DateTime CreationTime) : IEvent
{
    [JsonIgnore]
    public Guid Id { get; } = Id;

    [JsonIgnore]
    public DateTime CreationTime { get; } = CreationTime;

    public Event() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
