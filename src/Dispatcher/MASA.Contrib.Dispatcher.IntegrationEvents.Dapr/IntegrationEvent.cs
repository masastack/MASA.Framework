namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork UnitOfWork { get; set; }

    public abstract string Topic { get; set; }

    public IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public IntegrationEvent(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
