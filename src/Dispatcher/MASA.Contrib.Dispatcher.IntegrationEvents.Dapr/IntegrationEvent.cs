namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public abstract record IntegrationEvent : IIntegrationEvent
{
    [JsonIgnore]
    public Guid Id { get; init; }

    [JsonIgnore]
    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    [JsonIgnore]
    public abstract string Topic { get; set; }

    public IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public IntegrationEvent(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
