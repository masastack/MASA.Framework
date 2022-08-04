// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.CheckMethodsParameter.EventHandlers;

public class AddGoodsHandler
{
    [EventHandler]
    public void AddGoods(AddGoodsEvent @event, ILogger<AddGoodsHandler>? logger)
    {
        logger?.LogInformation($"add goods log,GoodsId:{@event.GoodsId},GoodsName:{@event.GoodsName},CategoryId:{@event.CategoryId}");
    }
}
