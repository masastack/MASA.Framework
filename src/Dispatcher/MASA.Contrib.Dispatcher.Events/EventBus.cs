namespace MASA.Contrib.Dispatcher.Events;

public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Internal.Dispatch.Dispatcher _dispatcher;

    private readonly DispatcherOptions _options;

    private IUnitOfWork? _unitOfWork;

    public EventBus(IServiceProvider serviceProvider, IOptions<DispatcherOptions> options)
    {
        _serviceProvider = serviceProvider;
        _dispatcher = serviceProvider.GetRequiredService<Internal.Dispatch.Dispatcher>();
        _options = options.Value;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var middlewares = _serviceProvider.GetRequiredService<IEnumerable<IMiddleware<TEvent>>>();
        if (@event is ITransaction transactionEvent)
        {
            transactionEvent.UnitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            if (_unitOfWork is null)
            {
                _unitOfWork = transactionEvent.UnitOfWork;
            }
            else
            {
                middlewares = middlewares.Where(middleware => middleware is not TransactionMiddleware<TEvent>);
            }
        }

        EventHandlerDelegate publishEvent = async () =>
        {
            await _dispatcher.PublishEventAsync(_serviceProvider, @event);
        };
        await middlewares.Reverse().Aggregate(publishEvent, (next, middleware) => () => middleware.HandleAsync(@event, next))();
    }

    public IEnumerable<Type> GetAllEventTypes() => _options.GetAllEventTypes();
}
