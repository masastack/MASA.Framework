namespace MASA.Contribs.DDD.Domain.Events;

public class DomainCommand : IDomainCommand
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork UnitOfWork { get; set; }

    public DomainCommand() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainCommand(Guid id, DateTime creationTime)
    {
        Id = id;
        CreationTime = creationTime;
    }

    public override string ToString()
    {
        return $"Id:{Id}, CreationTime:{CreationTime}";
    }
}
