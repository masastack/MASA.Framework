// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events;

public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Internal.Dispatch.Dispatcher _dispatcher;

    private readonly DispatcherOptions _options;

    private readonly IUnitOfWork? _unitOfWork;

    private readonly string LoadEventHelpLink = "https://github.com/masastack/Masa.Contrib/tree/main/docs/LoadEvent.md";

    public readonly IInitializeServiceProvider _initializeServiceProvider;

    public EventBus(IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        IInitializeServiceProvider initializeServiceProvider,
        IUnitOfWork? unitOfWork = null)
    {
        _serviceProvider = serviceProvider;
        _dispatcher = serviceProvider.GetRequiredService<Internal.Dispatch.Dispatcher>();
        _options = options.Value;
        _initializeServiceProvider = initializeServiceProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(@event, nameof(@event));
        var eventType = @event.GetType();

        var middlewares = _serviceProvider.GetRequiredService<IEnumerable<IMiddleware<TEvent>>>();
        if (!_options.UnitOfWorkRelation.ContainsKey(eventType))
        {
            throw new NotSupportedException(
                $"Getting \"{eventType.Name}\" relationship chain failed, see {LoadEventHelpLink} for details. ");
        }

        if (_options.UnitOfWorkRelation[eventType])
        {
            ITransaction transactionEvent = (ITransaction)@event;
            transactionEvent.UnitOfWork = _unitOfWork;
        }

        if (_initializeServiceProvider.IsInitialize)
            middlewares = middlewares.Where(middleware => middleware.SupportRecursive);

        _initializeServiceProvider.Initialize();

        EventHandlerDelegate eventHandlerDelegate = async () =>
        {
            await _dispatcher.PublishEventAsync(_serviceProvider, @event);
        };
        await middlewares.Reverse().Aggregate(eventHandlerDelegate, (next, middleware) => () => middleware.HandleAsync(@event, next))();
    }

    public IEnumerable<Type> GetAllEventTypes() => _options.AllEventTypes;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork is null)
            throw new ArgumentNullException("You need to UseUoW when adding services");

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
