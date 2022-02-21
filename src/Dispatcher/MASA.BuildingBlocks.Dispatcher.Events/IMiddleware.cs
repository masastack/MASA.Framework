namespace MASA.BuildingBlocks.Dispatcher.Events;

public delegate Task EventHandlerDelegate();

/// <summary>
/// Middleware is assembled into an event pipeline to handle invoke event and result
/// </summary>
public interface IMiddleware<TEvent>
    where TEvent : notnull, IEvent
{
    Task HandleAsync(TEvent @event, EventHandlerDelegate next);
}
