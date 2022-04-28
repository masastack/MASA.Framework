// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EF;

public static class DispatcherOptionsExtensions
{
    public static IEventBusBuilder UseIsolationUoW<TDbContext>(
        this IEventBusBuilder eventBusBuilder,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
        => eventBusBuilder.UseIsolationUoW<TDbContext, Guid>(isolationBuilder, optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IEventBusBuilder UseIsolationUoW<TDbContext, TKey>(
        this IEventBusBuilder eventBusBuilder,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
        where TKey : IComparable
    {
        eventBusBuilder.Services.UseIsolationUoW<TKey>();
        return eventBusBuilder.UseIsolation(isolationBuilder)
                              .UseUoW<TDbContext>(optionsBuilder, disableRollbackOnFailure, useTransaction);
    }

    public static IDispatcherOptions UseIsolationUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
        => options.UseIsolationUoW<TDbContext, Guid>(isolationBuilder, optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IDispatcherOptions UseIsolationUoW<TDbContext, TKey>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
        where TKey : IComparable
    {
        options.Services.UseIsolationUoW<TKey>();
        return options.UseIsolation(isolationBuilder)
                      .UseUoW<TDbContext>(optionsBuilder, disableRollbackOnFailure, useTransaction);
    }

    private static void UseIsolationUoW<TKey>(this IServiceCollection services) where TKey : IComparable
        => services.UseIsolationUoW()
                   .TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter), typeof(IsolationSaveChangesFilter<TKey>), ServiceLifetime.Scoped));

    private static IServiceCollection UseIsolationUoW(this IServiceCollection services)
    {
        if (services.Any(service => service.ServiceType == typeof(IConnectionStringProvider)))
            services.Replace(new ServiceDescriptor(typeof(IConnectionStringProvider), typeof(DefaultConnectionStringProvider), ServiceLifetime.Scoped));
        else
            services.TryAddScoped<IConnectionStringProvider, DefaultConnectionStringProvider>();
        return services;
    }

    private class IsolationUoWProvider
    {
    }
}
