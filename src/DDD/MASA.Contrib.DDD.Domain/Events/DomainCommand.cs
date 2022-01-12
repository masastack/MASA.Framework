namespace MASA.Contrib.DDD.Domain.Events;

public record DomainCommand : IDomainCommand
{
    [JsonIgnore]
    public Guid Id { get; init; }

    [JsonIgnore]
    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public DomainCommand() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainCommand(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
