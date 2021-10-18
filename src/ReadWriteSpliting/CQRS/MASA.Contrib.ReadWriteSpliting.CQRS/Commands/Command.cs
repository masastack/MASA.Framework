namespace MASA.Contrib.ReadWriteSpliting.CQRS.Commands;

public record Command : ICommand
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public Command() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Command(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
