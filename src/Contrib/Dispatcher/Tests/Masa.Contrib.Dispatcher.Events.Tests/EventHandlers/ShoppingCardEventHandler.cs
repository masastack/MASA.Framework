// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class ShoppingCardEventHandler
{
    private readonly ILogger<ShoppingCardEventHandler> _logger;
    public ShoppingCardEventHandler(ILogger<ShoppingCardEventHandler> logger) => _logger = logger;

    [EventHandler(FailureLevels = FailureLevels.Ignore)]
    public void AddShoppingCard(AddShoppingCartEvent @event)
    {
        _logger.LogInformation($"add shopping card log，GoodsId:{@event.GoodsId},Count：{@event.Count}");
        throw new ArgumentException(nameof(@event));
    }
}
