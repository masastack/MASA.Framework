// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIsolationDbContext<TDbContextImplementation>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : DbContext, IMasaDbContext
        => services.AddMasaDbContext<TDbContextImplementation>(optionsBuilder, contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation>(optionsLifetime);

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        if (services.Any(service => service.ImplementationType == typeof(IsolationDbContextProvider<TDbContextImplementation>)))
            return services;

        return services
            .AddSingleton<IsolationDbContextProvider<TDbContextImplementation>>()
            .AddIsolationFilter<TDbContextImplementation>(optionsLifetime)
            .AddConnectionStringProvider();
    }

    private static IServiceCollection AddIsolationFilter<TDbContextImplementation>(
        this IServiceCollection services,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>), serviceProvider =>
        {
            var isolationOptions = serviceProvider.GetService<IOptions<IsolationOptions>>();
            if (isolationOptions == null || !isolationOptions.Value.Enable)
            {
                return new EmptySaveFilter<TDbContextImplementation>();
            }

            var genericType = typeof(IsolationSaveChangesFilter<,>).MakeGenericType(typeof(TDbContextImplementation),
                isolationOptions.Value.MultiTenantIdType);
            var isolationSaveChangesFilter = Activator.CreateInstance(genericType, serviceProvider);
            return (isolationSaveChangesFilter as ISaveChangesFilter<TDbContextImplementation>)!;
        }, optionsLifetime));
        return services;
    }

    private static IServiceCollection AddConnectionStringProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IIsolationConnectionStringProviderWrapper>(serviceProvider =>
            new DefaultIsolationConnectionStringProvider(
                serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>(),
                serviceProvider.GetRequiredService<IIsolationConfigProvider>(),
                serviceProvider.GetService<IUnitOfWorkAccessor>(),
                serviceProvider.GetService<IMultiEnvironmentContext>(),
                serviceProvider.GetService<IMultiTenantContext>(),
                serviceProvider.GetService<ILogger<DefaultIsolationConnectionStringProvider>>()));
        services.Remove(ServiceDescriptor.Scoped(typeof(IConnectionStringProvider), serviceProvider =>
        {
            if (serviceProvider.EnableIsolation())
                return serviceProvider.GetServices(typeof(IIsolationConnectionStringProviderWrapper));

            return serviceProvider.GetServices(typeof(IConnectionStringProviderWrapper));
        }));
        return services;
    }

#pragma warning disable S2326
#pragma warning disable S2094
    private sealed class IsolationDbContextProvider<TDbContext>
    {

    }
#pragma warning restore S2094
#pragma warning restore S2326
}
