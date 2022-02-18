namespace MASA.Contrib.ReadWriteSpliting.CQRS.Commands;

public record Command(Guid Id, DateTime CreationTime) : ICommand
{
    [JsonIgnore]
    public Guid Id { get; } = Id;

    [JsonIgnore]
    public DateTime CreationTime { get; } = CreationTime;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public Command() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
