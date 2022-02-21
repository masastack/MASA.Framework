namespace MASA.BuildingBlocks.Dispatcher.Events;
public interface ISagaEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
{
    Task CancelAsync(TEvent @event);
}