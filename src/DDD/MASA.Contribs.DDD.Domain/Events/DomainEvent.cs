namespace MASA.Contribs.DDD.Domain.Events;

public class DomainEvent : IEvent, IDomainEvent
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork UnitOfWork { get; set; }

    public DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainEvent(Guid id, DateTime creationTime)
    {
        Id = id;
        CreationTime = creationTime;
    }

    public override string ToString()
    {
        return $"Id:{Id}, CreationTime:{CreationTime}";
    }
}


