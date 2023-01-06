// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public static class EventBusBuilderExtensions
{
    /// <summary>
    /// It is not recommended to use directly here, please use UseIsolationUoW
    /// </summary>
    /// <param name="eventBusBuilder"></param>
    /// <param name="isolationBuilder"></param>
    /// <returns></returns>
    public static IEventBusBuilder UseIsolation(this IEventBusBuilder eventBusBuilder, Action<IsolationBuilder> isolationBuilder)
    {
        eventBusBuilder.Services.AddIsolation(isolationBuilder);
        return eventBusBuilder;
    }
}
