// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaller(
        this IServiceCollection services,
        Action<CallerOptionsBuilder> configure)
        => services.AddCaller(configure, null);
        MasaArgumentException.ThrowIfNull(name);

    public static IServiceCollection AddCaller(
        this IServiceCollection services,
        Action<CallerOptionsBuilder> configure,
        ServiceLifetime? factoryLifetime)
        => services.AddCaller(Microsoft.Extensions.Options.Options.DefaultName, configure, factoryLifetime);

    public static IServiceCollection AddCaller(
        this IServiceCollection services,
        string name,
        Action<CallerOptionsBuilder> configure)
        => services.AddCaller(name, configure, null);

    public static IServiceCollection AddCaller(
        this IServiceCollection services,
        string name,
        Action<CallerOptionsBuilder> configure,
        ServiceLifetime? factoryLifetime)
    {
        MasaArgumentException.ThrowIfNull(services);

        services.AddCallerCore();

        var optionsBuilder = new CallerOptionsBuilder(services, name, factoryLifetime);
        configure.Invoke(optionsBuilder);

        return services;
    }

    public static IServiceCollection AddAutoRegistrationCaller(
        this IServiceCollection services,
        ServiceLifetime callerLifetime = ServiceLifetime.Scoped,
        ServiceLifetime? factoryLifetime = null)
        => services.AddAutoRegistrationCaller(MasaApp.GetAssemblies(), callerLifetime, factoryLifetime);

    public static IServiceCollection AddAutoRegistrationCaller(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime callerLifetime = ServiceLifetime.Scoped,
        ServiceLifetime? factoryLifetime = null)
    {
        MasaArgumentException.ThrowIfNull(services);

        services.AddCallerCore();
        services.AddAutomaticCaller(assemblies, callerLifetime, factoryLifetime);
        return services;
    }

    public static IServiceCollection AddAutoRegistrationCaller(
        this IServiceCollection services,
        params Assembly[] assemblies)
        => services.AddAutoRegistrationCaller(assemblies.AsEnumerable());

    private static void AddCallerCore(this IServiceCollection services)
    {
        services.TryAddTransient<ICallerFactory, DefaultCallerFactory>();
        services.TryAddTransient<ICaller>(serviceProvider => serviceProvider.GetRequiredService<ICallerFactory>().Create());
        services.TryAddSingleton<IRequestMessage>(_ => new JsonRequestMessage());
        services.TryAddSingleton<IResponseMessage>(_ => new JsonResponseMessage());
        
        services.TryAddSingleton<ITypeConvertor, DefaultTypeConvertor>();
        services.AddServiceFactory();
    }

    private static void AddAutomaticCaller(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime callerLifetime,
        ServiceLifetime? factoryLifetime)
    {
        var callerTypes = assemblies.SelectMany(x => x.GetTypes())
            .Where(type => typeof(CallerBase).IsAssignableFrom(type) && !type.IsAbstract).ToList();

        callerTypes = callerTypes.Except(services.Select(d => d.ServiceType)).ToList();

        if (callerTypes.Count == 0)
            return;

#pragma warning disable S3011
        callerTypes.Arrangement().ForEach(type =>
        {
            var serviceDescriptor = new ServiceDescriptor(type, serviceProvider =>
            {
                var constructorInfo = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .MaxBy(constructor => constructor.GetParameters().Length)!;
                List<object?> parameters = new();
                foreach (var parameter in constructorInfo.GetParameters())
                {
                    parameters.Add(serviceProvider.GetService(parameter.ParameterType));
                }
                var callerBase = (constructorInfo.Invoke(parameters.ToArray()) as CallerBase)!;

                var name = callerBase.Name ?? type.FullName ?? type.Name;
                callerBase.SetCallerOptions(new CallerOptionsBuilder(services, name, factoryLifetime), name);
                callerBase.Initialize(serviceProvider, type);

                return callerBase;
            }, callerLifetime);
            services.TryAdd(serviceDescriptor);
        });
#pragma warning disable S3011

        var serviceProvider = services.BuildServiceProvider();
        callerTypes.ForEach(type =>
        {
            var callerBase = (CallerBase)serviceProvider.GetRequiredService(type);
            callerBase.UseCallerExtension();
        });
    }
}
