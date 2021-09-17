namespace MASA.Contrib.Dispatcher.InMemory;

public class Event : IEvent
{
    public Event()
    {
        this.Id = Guid.NewGuid();
        this.CreationTime = DateTime.UtcNow;
    }

    public Guid Id { get; set; }

    public DateTime CreationTime { get; set; }

    public override string ToString()
    {
        return $"CreationTime:{CreationTime}, Id:{Id}";
    }
}