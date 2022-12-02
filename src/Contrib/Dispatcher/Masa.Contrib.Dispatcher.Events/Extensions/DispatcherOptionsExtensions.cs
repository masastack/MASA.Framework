// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Dispatcher.Events;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options)
        => options.UseEventBus(ServiceLifetime.Scoped);

    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options,
        Action<EventBusBuilder> eventBusBuilder)
        => options.UseEventBus(eventBusBuilder, ServiceLifetime.Scoped);

    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options,
        ServiceLifetime lifetime)
        => options.UseEventBus(null, lifetime);

    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options,
        Action<EventBusBuilder>? eventBusBuilder,
        ServiceLifetime lifetime)
    {
        ArgumentNullException.ThrowIfNull(options.Services, nameof(options.Services));

        options.Services.AddEventBus(options.Assemblies, lifetime, eventBusBuilder);
        return options;
    }
}
