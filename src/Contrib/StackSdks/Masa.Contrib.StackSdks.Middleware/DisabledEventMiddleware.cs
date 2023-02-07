// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware;

internal class DisabledEventMiddleware<TEvent> : EventMiddleware<TEvent>
    where TEvent : notnull, IEvent
{
    readonly ILogger<DisabledEventMiddleware<TEvent>> _logger;
    readonly IDisabledEventDeterminer _disabledEventDeterminer;

    public DisabledEventMiddleware(
        ILogger<DisabledEventMiddleware<TEvent>> logger,
        IDisabledEventDeterminer disabledEventDeterminer)
    {
        _logger = logger;
        _disabledEventDeterminer = disabledEventDeterminer;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        if (_disabledEventDeterminer.Determiner() && (_disabledEventDeterminer.DisabledCommand && @event is ICommand))
        {
            var allowedEventAttribute = Attribute.GetCustomAttribute(typeof(TEvent), typeof(AllowedEventAttribute));
            if (allowedEventAttribute == null)
            {
                _logger.LogWarning("disabled event operation");
                //throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.GUEST_ACCOUNT_OPERATE);
            }
        }
        await next();
    }
}
