namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public abstract record IntegrationEvent(Guid Id, DateTime CreationTime) : IIntegrationEvent
{
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    [JsonIgnore]
    public abstract string Topic { get; set; }

    public IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
