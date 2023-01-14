// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaller(this IServiceCollection services, params Assembly[] assemblies)
        => services.AddCaller(options => options.Assemblies = assemblies);

    public static IServiceCollection AddCaller(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        IEnumerable<Assembly>? assemblies = null)
        => services.AddCaller(options =>
        {
            options.Assemblies = assemblies?.ToArray() ?? MasaApp.GetAssemblies();
            options.CallerLifetime = lifetime;
        });

    public static IServiceCollection AddCaller(this IServiceCollection services, Action<CallerOptions> options)
    {
        CallerOptions callerOption = new CallerOptions(services);
        options.Invoke(callerOption);

        services.TryAddSingleton<ICallerFactory, DefaultCallerFactory>();
        services.TryAddSingleton<IRequestMessage>(_ => new JsonRequestMessage(callerOption.JsonSerializerOptions));
        services.TryAddSingleton<IResponseMessage>(_ => new JsonResponseMessage(callerOption.JsonSerializerOptions));
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<ICallerFactory>().Create());

        services.TryAddSingleton<ITypeConvertor, DefaultTypeConvertor>();
        if (!callerOption.DisableAutoRegistration) services.AddAutomaticCaller(callerOption);
        TryOrUpdate(services, callerOption);
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    private static void AddAutomaticCaller(this IServiceCollection services, CallerOptions callerOptions)
    {
        var callerTypes = callerOptions.Assemblies.SelectMany(x => x.GetTypes())
            .Where(type => typeof(CallerBase).IsAssignableFrom(type) && !type.IsAbstract).ToList();

        callerTypes = callerTypes.Except(services.Select(d => d.ServiceType)).ToList();

        if (callerTypes.Count == 0)
            return;

#pragma warning disable S3011
        callerTypes.Arrangement().ForEach(type =>
        {
            var serviceDescriptor = new ServiceDescriptor(type, serviceProvider =>
            {
                var constructorInfo = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .MaxBy(constructor => constructor.GetParameters().Length)!;
                List<object?> parameters = new();
                foreach (var parameter in constructorInfo.GetParameters())
                {
                    parameters.Add(serviceProvider.GetService(parameter.ParameterType));
                }
                var callerBase = (constructorInfo.Invoke(parameters.ToArray()) as CallerBase)!;
                callerBase.SetCallerOptions(callerOptions, type.FullName ?? type.Name);
                if (callerBase.ServiceProvider == null)
                {
                    callerBase.SetServiceProvider(serviceProvider);
                }

                return callerBase;
            }, callerOptions.CallerLifetime);
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

    private static void TryOrUpdate(this IServiceCollection services, CallerOptions options)
    {
        services.Configure<CallerFactoryOptions>(callerOptions =>
        {
            options.Callers.ForEach(caller =>
            {
                if (callerOptions.Options.Any(relation => relation.Name == caller.Name))
                    throw new ArgumentException(
                        $"The caller name already exists, please change the name, the repeat name is [{caller.Name}]");

                callerOptions.Options.Add(caller);
            });
        });
    }
}
