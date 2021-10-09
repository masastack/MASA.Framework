namespace MASA.Contribs.DDD.Domain;

public class DomainEventBus : IDomainEventBus
{
    protected readonly IEventBus _eventBus;
    protected readonly IIntegrationEventBus _integrationEventBus;
    private readonly IUnitOfWork _unitOfWork;

    private readonly ConcurrentQueue<IDomainEvent> _eventQueue = new ConcurrentQueue<IDomainEvent>();

    public DomainEventBus(IEventBus eventBus, IIntegrationEventBus integrationEventBus, IUnitOfWork unitOfWork)
    {
        _eventBus = eventBus;
        _integrationEventBus = integrationEventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        if (@event is IIntegrationEvent integrationEvent)
        {
            if (integrationEvent.UnitOfWork == null)
            {
                integrationEvent.UnitOfWork = _unitOfWork;
            }
            await _integrationEventBus.PublishAsync(integrationEvent);
        }
        else
        {
            await _eventBus.PublishAsync(@event);
        }
    }

    public Task Enqueue<TDomentEvent>(TDomentEvent @event) where TDomentEvent : IDomainEvent
    {
        _eventQueue.Enqueue(@event);
        return Task.CompletedTask;
    }

    public async Task PublishQueueAsync()
    {
        while (_eventQueue.TryDequeue(out IDomainEvent? @event))
        {
            await PublishAsync(@event);
        }
    }

    public IEnumerable<Type> GetAllEventTypes() => _eventBus.GetAllEventTypes();
}
