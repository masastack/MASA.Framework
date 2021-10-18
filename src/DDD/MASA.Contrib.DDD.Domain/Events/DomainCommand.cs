namespace MASA.Contrib.DDD.Domain.Events;

public record DomainCommand : IDomainCommand
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork UnitOfWork { get; set; }

    public DomainCommand() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainCommand(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
