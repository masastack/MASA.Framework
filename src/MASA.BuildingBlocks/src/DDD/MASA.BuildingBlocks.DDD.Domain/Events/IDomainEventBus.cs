namespace MASA.BuildingBlocks.DDD.Domain.Events;
public interface IDomainEventBus : IEventBus
{
    Task Enqueue<TDomentEvent>(TDomentEvent @event)
        where TDomentEvent : IDomainEvent;

    Task PublishQueueAsync();
}
