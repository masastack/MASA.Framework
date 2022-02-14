namespace MASA.Contrib.DDD.Domain.Events;

public record DomainCommand(Guid Id, DateTime CreationTime) : IDomainCommand
{
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public DomainCommand() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
