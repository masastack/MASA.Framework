// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EF;

public static class DispatcherOptionsExtensions
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
        => eventBusBuilder.UseIsolationUoW<TDbContext, TTenantId, TTenantId>(isolationBuilder, optionsBuilder, disableRollbackOnFailure,
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
        eventBusBuilder.Services.UseIsolationUoW<TTenantId>();
        return eventBusBuilder.UseIsolation(isolationBuilder)
            .UseUoW<TDbContext, TUserId>(optionsBuilder, disableRollbackOnFailure, useTransaction);
    }

    public static IDispatcherOptions UseIsolationUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        => options.UseIsolationUoW<TDbContext, Guid>(isolationBuilder, optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IDispatcherOptions UseIsolationUoW<TDbContext, TTenantId>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TTenantId : IComparable
        => options.UseIsolationUoW<TDbContext, TTenantId, TTenantId>(isolationBuilder, optionsBuilder, disableRollbackOnFailure,
            useTransaction);

    public static IDispatcherOptions UseIsolationUoW<TDbContext, TTenantId, TUserId>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TTenantId : IComparable
        where TUserId : IComparable
    {
        options.Services.UseIsolationUoW<TTenantId>();
        return options.UseIsolation(isolationBuilder)
            .UseUoW<TDbContext, TUserId>(optionsBuilder, disableRollbackOnFailure, useTransaction);
    }

    private static void UseIsolationUoW<TTenantId>(this IServiceCollection services) where TTenantId : IComparable
        => services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter), typeof(IsolationSaveChangesFilter<TTenantId>),
            ServiceLifetime.Scoped));
}
