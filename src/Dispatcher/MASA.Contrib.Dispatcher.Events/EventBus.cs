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
        if (@event is null)
        {
            throw new ArgumentNullException(typeof(TEvent).Name);
        }

        var middlewares = _serviceProvider.GetRequiredService<IEnumerable<IMiddleware<TEvent>>>();
        if (@event is ITransaction transactionEvent)
        {
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            if (unitOfWork != null)
            {
                transactionEvent.UnitOfWork = unitOfWork;
                if (_unitOfWork is null)
                {
                    _unitOfWork = transactionEvent.UnitOfWork;
                }
                else
                {
                    middlewares = middlewares.Where(middleware => middleware is not TransactionMiddleware<TEvent>);
                }
            }
        }

        EventHandlerDelegate publishEvent = async () =>
        {
            await _dispatcher.PublishEventAsync(_serviceProvider, @event);
        };
        await middlewares.Reverse().Aggregate(publishEvent, (next, middleware) => () => middleware.HandleAsync(@event, next))();
    }

    public IEnumerable<Type> GetAllEventTypes() => _options.AllEventTypes;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork is null)
            throw new ArgumentNullException("You need to UseUoW when adding services");

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
