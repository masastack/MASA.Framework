namespace MASA.Contrib.ReadWriteSpliting.CQRS.Commands;

public record Command(Guid Id, DateTime CreationTime) : ICommand
{
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public Command() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
