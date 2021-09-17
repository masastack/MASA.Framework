namespace MASA.Contrib.Dispatcher.InMemory;

public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dispatch.Dispatcher _dispatcher;

    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _dispatcher = serviceProvider.GetRequiredService<Dispatch.Dispatcher>();
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var pipelines = _serviceProvider.GetRequiredService<IEnumerable<IMiddleware<TEvent>>>();
        EventHandlerDelegate publishEvent = async () => await _dispatcher.PublishEventAsync(_serviceProvider, @event);
        await pipelines.Reverse().Aggregate(publishEvent, (next, pipeline) => () => pipeline.HandleAsync(@event, next))();
    }
}