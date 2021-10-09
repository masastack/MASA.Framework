namespace MASA.Contrib.ReadWriteSpliting.CQRS.Commands;

public class Command : ICommand
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public Command() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Command(Guid id, DateTime creationTime)
    {
        Id = id;
        CreationTime = creationTime;
    }

    public override string ToString()
    {
        return $"Id:{Id}, CreationTime:{CreationTime}";
    }
}
