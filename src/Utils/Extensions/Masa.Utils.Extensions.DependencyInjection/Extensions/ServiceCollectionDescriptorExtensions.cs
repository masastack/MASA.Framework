// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionDescriptorExtensions
{
    public static TService GetInstance<TService>(this IServiceCollection services, bool isCreateScope = false)
        where TService : class
    {
        if (isCreateScope)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            return scope.ServiceProvider.GetInstance<TService>();
        }

        return services.BuildServiceProvider().GetInstance<TService>();
    }

    private static TService GetInstance<TService>(this IServiceProvider serviceProvider)
        where TService : class
    {
        if (typeof(TService) == typeof(IServiceProvider))
            return (TService)serviceProvider;

        return serviceProvider.GetRequiredService<TService>();
    }

    /// <summary>
    /// Returns whether the specified ServiceType exists in the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public static bool Any<TService>(this IServiceCollection services)
        => services.Any(d => d.ServiceType == typeof(TService));

    /// <summary>
    /// Returns whether the specified ServiceType and ImplementationType exist in the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static bool Any<TService, TImplementation>(this IServiceCollection services)
        => services.Any(d => d.ServiceType == typeof(TService) && d.ImplementationType == typeof(TImplementation));

    /// <summary>
    /// Returns whether the specified ServiceType exists in the service collection, and the life cycle is the life cycle.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public static bool Any<TService>(this IServiceCollection services, ServiceLifetime lifetime)
        => services.Any(d => d.ServiceType == typeof(TService) && d.Lifetime == lifetime);

    /// <summary>
    /// Returns the specified ServiceType, ImplementationType, and whether the life cycle is lifetime exists in the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static bool Any<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        => services.Any(d => d.ServiceType == typeof(TService) && d.ImplementationType == typeof(TImplementation) && d.Lifetime == lifetime);

    /// <summary>
    /// Remove the first service in the service collection with the same service type and add the implementationType to the collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="implementationType"></param>
    /// <param name="lifetime"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public static IServiceCollection Replace<TService>(this IServiceCollection services, Type implementationType, ServiceLifetime lifetime)
    {
        if (services.Any<TService>())
        {
            int count = services.Count;
            for (int i = 0; i < count; i++)
            {
                if (services[i].ServiceType == typeof(TService))
                {
                    services.RemoveAt(i);
                    break;
                }
            }
        }

        services.Add(new ServiceDescriptor(typeof(TService), implementationType, lifetime));
        return services;
    }

    /// <summary>
    /// Removes all services with the same service type in the services collection and adds implementationType to the collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="implementationType"></param>
    /// <param name="lifetime"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public static IServiceCollection ReplaceAll<TService>(this IServiceCollection services, Type implementationType, ServiceLifetime lifetime)
    {
        if (services.Any<TService>())
        {
            int count = services.Count;
            for (int i = 0; i < count; i++)
            {
                if (services[i].ServiceType == typeof(TService))
                {
                    services.RemoveAt(i);
                }
            }
        }

        services.Add(new ServiceDescriptor(typeof(TService), implementationType, lifetime));
        return services;
    }
}
