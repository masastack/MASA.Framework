// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.UoW.EFCore")]
// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

internal static class EventBusBuilderExtensions
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
