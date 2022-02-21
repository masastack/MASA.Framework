namespace MASA.BuildingBlocks.Dispatcher.Events;
public interface IEventHandler<TEvent>
        where TEvent : IEvent
{
    Task HandleAsync(TEvent @event);
}