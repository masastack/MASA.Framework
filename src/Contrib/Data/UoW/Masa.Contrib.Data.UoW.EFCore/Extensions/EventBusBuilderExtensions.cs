// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data.UoW;

public static class EventBusBuilderExtensions
{
    public static IEventBusBuilder UseUoW<TDbContext>(
        this IEventBusBuilder eventBusBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool? useTransaction = null)
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
    {
        eventBusBuilder.Services.UseUoW<TDbContext>(
            nameof(eventBusBuilder.Services),
            optionsBuilder,
            disableRollbackOnFailure,
            useTransaction);
        return eventBusBuilder;
    }
}
