// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoInject(this IServiceCollection services)
        => services.AddAutoInject(AppDomain.CurrentDomain.GetAssemblies());

    public static IServiceCollection AddAutoInject(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        => services.AddAutoInjectCore(assemblies);

    public static IServiceCollection AddAutoInject(this IServiceCollection services, params Assembly[] assemblies)
        => services.AddAutoInjectCore(assemblies);

    /// <summary>
    /// Automatic registration from an assembly containing the specified type
    /// </summary>
    /// <param name="services"></param>
    /// <param name="handlerAssemblyMarkerTypes"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoInject(this IServiceCollection services, IEnumerable<Type> handlerAssemblyMarkerTypes)
        => services.AddAutoInjectCore(handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly).Distinct().ToArray());

    /// <summary>
    /// Automatic registration from an assembly containing the specified type
    /// </summary>
    /// <param name="services"></param>
    /// <param name="handlerAssemblyMarkerTypes"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoInject(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
        => services.AddAutoInjectCore(handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly).Distinct().ToArray());

    private static IServiceCollection AddAutoInjectCore(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        if (services.Any<DependencyInjectionService>())
            return services;

        services.AddSingleton<DependencyInjectionService>();
        services.TryAddSingleton<IServiceRegister, DefaultServiceRegister>();
        services.TryAddSingleton<ITypeProvider, DefaultTypeProvider>();
        var typeProvider = services.GetInstance<ITypeProvider>();
        var serviceDescriptors = typeProvider.GetServiceDescriptors(typeProvider.GetAllTypes(assemblies));

        var registrar = services.GetInstance<IServiceRegister>();
        foreach (var descriptor in serviceDescriptors)
            registrar.Add(services, descriptor.ServiceType, descriptor.ImplementationType, descriptor.Lifetime);

        if (!serviceDescriptors.Any(d => d.AutoFire))
            return services;

        var serviceProvider = services.BuildServiceProvider();
        foreach (var descriptor in serviceDescriptors.Where(d => d.AutoFire))
            serviceProvider.GetService(descriptor.ServiceType);

        return services;
    }

    /// <summary>
    /// Auto add all service to IoC, lifecycle is scoped
    /// </summary>
    /// <param name="services"></param>
    /// <param name="suffix">default is Service</param>
    /// <param name="autoFire"></param>
    public static IServiceCollection AddServices(this IServiceCollection services, string suffix, bool autoFire)
        => services.AddServices(suffix, autoFire, Assembly.GetEntryAssembly()!);

    /// <summary>
    /// Auto add all service to IoC, lifecycle is scoped
    /// </summary>
    /// <param name="services"></param>
    /// <param name="suffix">default is Service</param>
    /// <param name="autoFire"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services, string suffix, bool autoFire,
        params Assembly[] assemblies)
        => (from type in assemblies.SelectMany(assembly => assembly.GetTypes())
            where !type.IsAbstract && type.Name.EndsWith(suffix)
            select type).AddScoped(services, autoFire);

    /// <summary>
    /// Auto add all service to IoC, lifecycle is scoped
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="services"></param>
    /// <param name="autoFire"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices<TService>(this IServiceCollection services, bool autoFire)
        => services.AddServices<TService>(autoFire, Assembly.GetEntryAssembly()!);

    /// <summary>
    /// Auto add all service to IoC, lifecycle is scoped
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="services"></param>
    /// <param name="autoFire"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices<TService>(this IServiceCollection services,
        bool autoFire,
        params Assembly[] assemblies)
        => (from type in assemblies.SelectMany(assembly => assembly.GetTypes())
            where !type.IsAbstract && BaseOf<TService>(type)
            select type).AddScoped(services, autoFire);

    /// <summary>
    /// Auto add all service to IoC, lifecycle is scoped
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="services"></param>
    /// <param name="autoFire"></param>
    /// <param name="action"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices<TService>(this IServiceCollection services,
        bool autoFire,
        Action<Type, object>? action,
        params Assembly[] assemblies)
        => (from type in assemblies.SelectMany(assembly => assembly.GetTypes())
            where !type.IsAbstract && BaseOf<TService>(type)
            select type).AddScoped(services, autoFire, action);

    private static IServiceCollection AddScoped(this IEnumerable<Type> serviceTypes,
        IServiceCollection services,
        bool autoFire,
        Action<Type, object>? action = null)
    {
        foreach (var serviceType in serviceTypes)
        {
            services.AddScoped(serviceType);
        }

        if (autoFire)
        {
            foreach (var serviceType in serviceTypes)
            {
                var service = services.BuildServiceProvider().GetService(serviceType);
                action?.Invoke(serviceType, service);
            }
        }

        return services;
    }

    private static bool BaseOf<T>(Type type)
    {
        if (type.BaseType == typeof(T)) return true;

        return type.BaseType != null && BaseOf<T>(type.BaseType);
    }

    private sealed class DependencyInjectionService
    {

    }
}
