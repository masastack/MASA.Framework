// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
        if (services.Any(service => service.ImplementationType == typeof(DomainEventBusProvider)))
            return services;

        services.AddSingleton<DomainEventBusProvider>();

        MasaArgumentException.ThrowIfNull(assemblies);

        var dispatcherOptions = new DispatcherOptions(services, assemblies.Distinct().ToArray());
        options?.Invoke(dispatcherOptions);
        services.AddSingleton(typeof(IOptions<DispatcherOptions>), _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        if (services.All(service => service.ServiceType != typeof(IEventBus)))
            throw new MasaException("Please add EventBus first.");

        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
            throw new MasaException("Please add UoW first.");

        if (services.All(service => service.ServiceType != typeof(IIntegrationEventBus)))
            throw new MasaException("Please add IntegrationEventBus first.");

        services.CheckAggregateRootRepositories(dispatcherOptions.AllAggregateRootTypes);

        foreach (var domainServiceType in dispatcherOptions.AllDomainServiceTypes)
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

        services.TryAddScoped<IDomainEventBus, DomainEventBus>();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

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

    private sealed class DomainEventBusProvider
    {

    }
}
