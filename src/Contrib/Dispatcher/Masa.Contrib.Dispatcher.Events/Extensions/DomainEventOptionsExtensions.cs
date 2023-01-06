// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.Events;

public static class DomainEventOptionsExtensions
{
    public static IDomainEventOptions UseEventBus(
        this IDomainEventOptions options)
        => options.UseEventBus(ServiceLifetime.Scoped);

    public static IDomainEventOptions UseEventBus(
        this IDomainEventOptions options,
        Action<EventBusBuilder> eventBusBuilder)
        => options.UseEventBus(eventBusBuilder, ServiceLifetime.Scoped);

    public static IDomainEventOptions UseEventBus(
        this IDomainEventOptions options,
        ServiceLifetime lifetime)
        => options.UseEventBus(null, lifetime);

    public static IDomainEventOptions UseEventBus(
        this IDomainEventOptions options,
        Action<EventBusBuilder>? eventBusBuilder,
        ServiceLifetime lifetime)
        => options.UseEventBus<IDomainEventOptions>(eventBusBuilder, lifetime);
}
