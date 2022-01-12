namespace MASA.Contrib.DDD.Domain.Events;

public record DomainEvent : IDomainEvent
{
    [JsonIgnore]
    public Guid Id { get; init; }

    [JsonIgnore]
    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainEvent(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
