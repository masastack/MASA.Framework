// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class LocalEventBusWrapper : ILocalEventBusWrapper
{
    private readonly Lazy<ILocalEventBus> _publisherLazy;

    private readonly DispatcherOptions _options;

    private readonly IUnitOfWork? _unitOfWork;

#pragma warning disable S5332
    private const string LOAD_EVENT_HELP_LINK =
        "https://docs.masastack.com/framework/building-blocks/dispatcher/faq#section-8fdb7a0b51854e8b4ef6";
#pragma warning restore S5332

    public LocalEventBusWrapper(IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        IUnitOfWork? unitOfWork = null)
    {
        _publisherLazy = new Lazy<ILocalEventBus>(serviceProvider.GetRequiredService<ILocalEventBus>);
        _options = options.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task PublishAsync<TEvent>(
        TEvent @event,
        IEnumerable<IEventMiddleware<TEvent>> eventMiddlewares,
        CancellationToken cancellationToken = default) where TEvent : IEvent
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

        EventHandlerDelegate eventHandlerDelegate = async () =>
        {
            await _publisherLazy.Value.ExecuteHandlerAsync(@event, cancellationToken);
        };
        await eventMiddlewares.Reverse().Aggregate(eventHandlerDelegate, (next, middleware) => () => middleware.HandleAsync(@event, next))();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        MasaArgumentException.ThrowIfNull(_unitOfWork);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
