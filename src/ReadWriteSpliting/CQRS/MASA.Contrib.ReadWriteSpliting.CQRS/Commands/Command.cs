namespace MASA.Contrib.ReadWriteSpliting.CQRS.Commands;

public record Command : ICommand
{
    [JsonIgnore]
    public Guid Id { get; init; }

    [JsonIgnore]
    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public Command() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Command(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
