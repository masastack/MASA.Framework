// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.CheckMethodsType.EventHandlers;

public class AddBasketHandler
{
    private readonly ILogger<AddBasketHandler>? _logger;
    public AddBasketHandler(ILogger<AddBasketHandler>? logger) => _logger = logger;

    [EventHandler]
    public Task<string> AddLog(AddBasketEvent @event)
    {
        _logger?.LogInformation($"add basket log：GoogdsId：{@event.GoodsId}，count：{@event.Count}");
        return Task.FromResult("success");
    }
}
