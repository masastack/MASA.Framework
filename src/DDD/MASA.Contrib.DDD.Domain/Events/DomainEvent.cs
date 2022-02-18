namespace MASA.Contrib.DDD.Domain.Events;

public record DomainEvent(Guid Id, DateTime CreationTime) : IDomainEvent
{
    [JsonIgnore]
    public Guid Id { get; } = Id;

    [JsonIgnore]
    public DateTime CreationTime { get; } = CreationTime;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

}
