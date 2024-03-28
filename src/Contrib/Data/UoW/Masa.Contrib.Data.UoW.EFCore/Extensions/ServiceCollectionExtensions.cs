// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection UseUoW<TDbContext>(
        this IServiceCollection services,
        string paramName,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool? useTransaction = null)
        where TDbContext : DefaultMasaDbContext, IMasaDbContext
    {
        MasaArgumentException.ThrowIfNull(services, paramName);

#if (NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(UoWProvider)))
            return services;
#else
        if (services.Any(service => service.ImplementationType == typeof(UoWProvider)))
            return services;
#endif

        services.AddSingleton<UoWProvider>();
        services.TryAddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager<TDbContext>>();

        services.AddScoped<IUnitOfWork>(serviceProvider => new UnitOfWork<TDbContext>(serviceProvider)
        {
            DisableRollbackOnFailure = disableRollbackOnFailure,
            UseTransaction = useTransaction
        });
        if (services.All(service => service.ServiceType != typeof(MasaDbContextOptions<TDbContext>)))
            services.AddMasaDbContext<TDbContext>(optionsBuilder);

        services.AddScoped<ITransaction, Transaction>();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    private sealed class UoWProvider
    {
    }
}
