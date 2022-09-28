// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore;

public static class DispatcherOptionsExtensions
{
    public static IEventBusBuilder UseUoW<TDbContext>(
        this IEventBusBuilder eventBusBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        => eventBusBuilder.UseUoW<TDbContext, Guid>(optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IEventBusBuilder UseUoW<TDbContext, TUserId>(
        this IEventBusBuilder eventBusBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        eventBusBuilder.Services.UseUoW<TDbContext, TUserId>(nameof(eventBusBuilder.Services), optionsBuilder, disableRollbackOnFailure,
            useTransaction);
        return eventBusBuilder;
    }

    public static IDispatcherOptions UseUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        => options.UseUoW<TDbContext, Guid>(optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IDispatcherOptions UseUoW<TDbContext, TUserId>(
        this IDispatcherOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        options.Services.UseUoW<TDbContext, TUserId>(nameof(options.Services), optionsBuilder, disableRollbackOnFailure, useTransaction);
        return options;
    }

    private static IServiceCollection UseUoW<TDbContext, TUserId>(
        this IServiceCollection services,
        string paramName,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        if (services == null)
            throw new ArgumentNullException(paramName);

        if (services.Any(service => service.ImplementationType == typeof(UoWProvider)))
            return services;

        services.AddSingleton<UoWProvider>();
        services.TryAddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager<TDbContext>>();
        services.TryAddScoped<IConnectionStringProvider, DefaultConnectionStringProvider>();

        services.AddScoped<IUnitOfWork>(serviceProvider => new UnitOfWork<TDbContext>(serviceProvider)
        {
            DisableRollbackOnFailure = disableRollbackOnFailure,
            UseTransaction = useTransaction
        });
        if (services.All(service => service.ServiceType != typeof(MasaDbContextOptions<TDbContext>)))
            services.AddMasaDbContext<TDbContext, TUserId>(optionsBuilder);

        services.AddScoped<ITransaction, Transaction>();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    private sealed class UoWProvider
    {
    }
}
