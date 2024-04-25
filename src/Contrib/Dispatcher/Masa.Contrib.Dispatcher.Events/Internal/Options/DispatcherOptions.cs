// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class DispatcherOptions
{
    private IServiceCollection Services { get; }

    private Assembly[] Assemblies { get; }

    private bool IsSupportUnitOfWork(Type eventType)
        => typeof(ITransaction).IsAssignableFrom(eventType) && !eventType.IsImplementerOfGeneric(typeof(IDomainQuery<>));

    internal Dictionary<Type, bool> UnitOfWorkRelation { get; } = new();

    private DispatcherOptions(IServiceCollection services) => Services = services;

    public DispatcherOptions(IServiceCollection services, Assembly[] assemblies)
        : this(services)
    {
        Assemblies = assemblies;

        var allEventTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();

        allEventTypes.AddRange(GetGenericTypeEventType(assemblies));

        UnitOfWorkRelation = allEventTypes.ToDictionary(type => type, IsSupportUnitOfWork);
    }

    private List<Type> GetGenericTypeEventType(Assembly[] assemblies)
    {
        var methods = assemblies
            .SelectMany(assembly => assembly.GetTypes().SelectMany(method => method.GetMethods()))
            .Where(method => method.GetCustomAttributes(typeof(EventHandlerAttribute), false).Length > 0);

        var allEventTypes = methods.SelectMany(method => method.GetParameters().Where(type => type.ParameterType.IsGenericType == true &&
                                type.ParameterType.GetGenericTypeDefinition()?.BaseType?.Name == typeof(EntityChangedEvent<>).Name))
            .Select(type => type.ParameterType).ToList();

        return allEventTypes;
    }
}
