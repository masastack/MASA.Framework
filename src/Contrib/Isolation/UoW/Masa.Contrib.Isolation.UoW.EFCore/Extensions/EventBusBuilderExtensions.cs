// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public static class EventBusBuilderExtensions
{
    public static IEventBusBuilder UseIsolationUoW<TDbContext>(
        this IEventBusBuilder eventBusBuilder,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        => eventBusBuilder.UseIsolationUoW<TDbContext, Guid>(isolationBuilder, optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IEventBusBuilder UseIsolationUoW<TDbContext, TTenantId>(
        this IEventBusBuilder eventBusBuilder,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TTenantId : IComparable
        => eventBusBuilder.UseIsolationUoW<TDbContext, TTenantId, TTenantId>(
            isolationBuilder,
            optionsBuilder,
            disableRollbackOnFailure,
            useTransaction);

    public static IEventBusBuilder UseIsolationUoW<TDbContext, TTenantId, TUserId>(
        this IEventBusBuilder eventBusBuilder,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TTenantId : IComparable
        where TUserId : IComparable
    {
        eventBusBuilder.Services.UseIsolationUoW<TDbContext, TTenantId>();
        return eventBusBuilder.UseIsolation(isolationBuilder)
            .UseUoW<TDbContext, TUserId>(optionsBuilder, disableRollbackOnFailure, useTransaction);
    }
}
