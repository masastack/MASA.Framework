// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class LocalEventBus : ILocalEventBus
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Lazy<LocalEventBusPublisher> _publisherLazy;

    private readonly DispatcherOptions _options;

    private readonly IUnitOfWork? _unitOfWork;

#pragma warning disable S5332
    private const string LOAD_EVENT_HELP_LINK = "https://docs.masastack.com/framework/building-blocks/dispatcher/faq#section-8fdb7a0b51854e8b4ef6";
#pragma warning restore S5332

    private readonly IInitializeServiceProvider _initializeServiceProvider;

    public LocalEventBus(IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        IInitializeServiceProvider initializeServiceProvider,
        IUnitOfWork? unitOfWork = null)
    {
        _serviceProvider = serviceProvider;
        _publisherLazy = new Lazy<LocalEventBusPublisher>(serviceProvider.GetRequiredService<LocalEventBusPublisher>);
        _options = options.Value;
        _initializeServiceProvider = initializeServiceProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        MasaArgumentException.ThrowIfNull(@event);
        var eventType = @event.GetType();
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

        var middlewares = _serviceProvider.GetRequiredService<IEnumerable<IEventMiddleware<TEvent>>>();
        if (_initializeServiceProvider.IsInitialize)
            middlewares = middlewares.Where(middleware => middleware.SupportRecursive);

        _initializeServiceProvider.Initialize();

        EventHandlerDelegate eventHandlerDelegate = async () =>
        {
            await _publisherLazy.Value.PublishEventAsync(@event, cancellationToken);
        };
        await middlewares.Reverse().Aggregate(eventHandlerDelegate, (next, middleware) => () => middleware.HandleAsync(@event, next))();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        MasaArgumentException.ThrowIfNull(_unitOfWork);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
