// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events;

public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Internal.Dispatch.Dispatcher _dispatcher;

    private readonly DispatcherOptions _options;

    private readonly IUnitOfWork? _unitOfWork;

#pragma warning disable S5332
    private const string LOAD_EVENT_HELP_LINK = "http://docs.masastack.com/framework/concepts/faq/load-event";
#pragma warning restore S5332

    private readonly IInitializeServiceProvider _initializeServiceProvider;

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

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(@event, nameof(@event));
        var eventType = @event.GetType();

        var middlewares = _serviceProvider.GetRequiredService<IEnumerable<IEventMiddleware<TEvent>>>();
        if (!_options.UnitOfWorkRelation.ContainsKey(eventType))
        {
            throw new NotSupportedException(
                $"Getting \"{eventType.Name}\" relationship chain failed, see {LOAD_EVENT_HELP_LINK} for details. ");
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
            await _dispatcher.PublishEventAsync(_serviceProvider, @event, cancellationToken);
        };
        await middlewares.Reverse().Aggregate(eventHandlerDelegate, (next, middleware) => () => middleware.HandleAsync(@event, next))();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork is null)
            throw new ArgumentNullException("You need to UseUoW when adding services");

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
