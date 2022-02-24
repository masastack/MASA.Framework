namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Events;

public record CreateUserEvent : IEvent
{
    public string Name { get; set; }

    public Guid Id { get; set; }

    public DateTime CreationTime { get; set; }

    public CreateUserEvent()
    {
        this.Id = Guid.NewGuid();
        this.CreationTime = DateTime.UtcNow;
    }

    public CreateUserEvent(string name) : this()
    {
        this.Name = name;
    }
}
