namespace MASA.Contrib.DDD.Domain.Events;

public record DomainEvent(Guid Id, DateTime CreationTime) : IDomainEvent
{
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
