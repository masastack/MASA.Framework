// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaller(this IServiceCollection services, Action<CallerOptionsBuilder> configure)
        => services.AddCaller(Microsoft.Extensions.Options.Options.DefaultName, configure);

    public static IServiceCollection AddCaller(this IServiceCollection services, string name, Action<CallerOptionsBuilder> configure)
    {
        MasaArgumentException.ThrowIfNull(services);

        services.AddCallerCore();

        var callerOption = new CallerOptionsBuilder(services, name);
        configure.Invoke(callerOption);

        return services;
    }

    public static IServiceCollection AddCaller(
        this IServiceCollection services,
        string name,
        Func<IServiceProvider, ICaller> implementationFactory)
    {
        MasaArgumentException.ThrowIfNull(services);
        MasaArgumentException.ThrowIfNull(name);

        services.Configure<CallerFactoryOptions>(callerOptions =>
        {
            if (callerOptions.Options.Any(relation => relation.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The caller name already exists, please change the name, the repeat name is [{name}]");

            callerOptions.Options.Add(new CallerRelationOptions(name, implementationFactory));
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    public static IServiceCollection AddAutoRegistrationCaller(
        this IServiceCollection services,
        ServiceLifetime callerLifetime = ServiceLifetime.Scoped)
        => services.AddAutoRegistrationCaller(MasaApp.GetAssemblies(), callerLifetime);

    public static IServiceCollection AddAutoRegistrationCaller(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime callerLifetime = ServiceLifetime.Scoped)
    {
        MasaArgumentException.ThrowIfNull(services);

        services.AddCallerCore();
        services.AddAutomaticCaller(assemblies, callerLifetime);
        return services;
    }

    public static IServiceCollection AddAutoRegistrationCaller(
        this IServiceCollection services,
        params Assembly[] assemblies)
        => services.AddAutoRegistrationCaller(assemblies.AsEnumerable());

    private static void AddCallerCore(this IServiceCollection services)
    {
        services.TryAddSingleton<ICallerFactory, DefaultCallerFactory>();
        services.TryAddSingleton<IRequestMessage>(_ => new JsonRequestMessage());
        services.TryAddSingleton<IResponseMessage>(_ => new JsonResponseMessage());
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<ICallerFactory>().Create());

        services.TryAddSingleton<ITypeConvertor, DefaultTypeConvertor>();
    }

    private static void AddAutomaticCaller(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime callerLifetime)
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
                callerBase.SetCallerOptions(new CallerOptionsBuilder(services, name), name);
                if (callerBase.ServiceProvider == null) callerBase.SetServiceProvider(serviceProvider);

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
