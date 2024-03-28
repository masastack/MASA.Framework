// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Ddd.Domain.Tests")]

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEventBus(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        => services.AddDomainEventBus(MasaApp.GetAssemblies(), options);

    public static IServiceCollection AddDomainEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<DispatcherOptions>? options = null)
    {

#if (NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(DomainEventBusProvider)))
            return services;
#else
        if (services.Any(service => service.ImplementationType == typeof(DomainEventBusProvider)))
            return services;
#endif

        services.AddSingleton<DomainEventBusProvider>();

        MasaArgumentException.ThrowIfNull(assemblies);

        var dispatcherOptions = new DispatcherOptions(services, assemblies.Distinct().ToArray());
        options?.Invoke(dispatcherOptions);
        services.AddSingleton(typeof(IOptions<DispatcherOptions>), _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        services.CheckRequiredService();

        services.CheckAggregateRootRepositories(dispatcherOptions.AllAggregateRootTypes);

        services.RegisterDomainService(dispatcherOptions.AllDomainServiceTypes);

        services.TryAddScoped<IDomainEventBus, DomainEventBus>();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    internal static void CheckRequiredService(this IServiceCollection services)
    {
        var isRegisterEventBus = services.Any(service => service.ServiceType == typeof(IEventBus));
        var isRegisterUnitOfWork = services.Any(service => service.ServiceType == typeof(IUnitOfWork));
        var isRegisterIntegrationEventBus = services.Any(service => service.ServiceType == typeof(IIntegrationEventBus));

        if (isRegisterEventBus && isRegisterUnitOfWork && isRegisterIntegrationEventBus)
            return;

        var logger = services.BuildServiceProvider().GetService<ILogger<DomainEventBusProvider>>();

        if (!isRegisterEventBus)
            logger?.LogWarning("Please add EventBus first.");

        if (!isRegisterUnitOfWork)
            logger?.LogWarning("Please add UoW first.");

        if (!isRegisterIntegrationEventBus)
            logger?.LogWarning("Please add IntegrationEventBus first.");
    }

    [ExcludeFromCodeCoverage]
    private static void CheckAggregateRootRepositories(this IServiceCollection services, List<Type> aggregateRootRepositoryTypes)
    {
        foreach (var aggregateRootRepositoryType in aggregateRootRepositoryTypes)
        {
            var serviceType = GetServiceRepositoryType(aggregateRootRepositoryType);
            if (services.All(service => service.ServiceType != serviceType))
                throw new NotImplementedException($"The number of types of {serviceType.FullName} implementation class must be 1.");
        }
    }

    private static Type GetServiceRepositoryType(Type entityType) => typeof(IRepository<>).MakeGenericType(entityType);

    internal static void RegisterDomainService(this IServiceCollection services, List<Type> domainServiceTypes)
    {
        foreach (var domainServiceType in domainServiceTypes)
        {
            var constructorInfo = domainServiceType
                .GetConstructors()
                .MaxBy(c => c.GetParameters().Length);
            MasaArgumentException.ThrowIfNull(constructorInfo);

            services.TryAddScoped(domainServiceType, serviceProvider =>
            {
                List<object?> parameters = new();
                foreach (var parameter in constructorInfo.GetParameters())
                {
                    parameters.Add(serviceProvider.GetService(parameter.ParameterType));
                }
                var domainServiceInstance = constructorInfo.Invoke(parameters.ToArray());
                var domainService = (domainServiceInstance as DomainService)!;
                if (domainService.EventBus == default!)
                {
                    domainService.SetDomainEventBus(serviceProvider.GetRequiredService<IDomainEventBus>());
                }
                return domainServiceInstance;
            });
        }
    }

    private sealed class DomainEventBusProvider
    {

    }
}
