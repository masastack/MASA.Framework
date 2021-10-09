using System.Text.Json.Serialization;

namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public abstract class IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public abstract string Topic { get; set; }

    [JsonIgnore]
    public IUnitOfWork UnitOfWork { get; set; }

    public IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public IntegrationEvent(Guid id, DateTime creationTime)
    {
        Id = id;
        CreationTime = creationTime;
    }

    public override string ToString()
    {
        return $"Id:{Id}, CreationTime:{CreationTime}";
    }
}
