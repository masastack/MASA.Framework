// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal;

internal static class DispatcherExtensions
{
    public static IServiceCollection Add(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
        return services;
    }

    public static IServiceCollection TryAdd(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        services.TryAdd(new ServiceDescriptor(serviceType, implementationType, lifetime));
        return services;
    }

    public static IServiceCollection TryAdd(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
    {
        services.TryAdd(new ServiceDescriptor(serviceType, factory, lifetime));
        return services;
    }

    public static bool IsGeneric(this Type type) => type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;

    public static bool IsConcrete(this Type type) => !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;

    public static bool IsGenericInterfaceAssignableFrom(this Type eventHandlerType, Type type) =>
        type.IsConcrete() &&
        type.GetInterfaces().Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == eventHandlerType);

    /// <summary>
    /// Keep the original stack information and throw an exception
    /// </summary>
    /// <param name="exception"></param>
    public static void ThrowException(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
