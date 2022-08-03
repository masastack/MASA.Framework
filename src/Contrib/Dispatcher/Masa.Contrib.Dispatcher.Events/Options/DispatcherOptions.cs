// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Options;

public class DispatcherOptions
{
    private IServiceCollection Services { get; }

    private Assembly[] Assemblies { get; }

    private bool IsSupportUnitOfWork(Type eventType)
        => typeof(ITransaction).IsAssignableFrom(eventType) && !typeof(IDomainQuery<>).IsGenericInterfaceAssignableFrom(eventType);

    internal Dictionary<Type, bool> UnitOfWorkRelation { get; } = new();

    public IEnumerable<Type> AllEventTypes { get; }

    private DispatcherOptions(IServiceCollection services) => Services = services;

    public DispatcherOptions(IServiceCollection services, Assembly[] assemblies)
        : this(services)
    {
        if (assemblies == null || assemblies.Length == 0)
            throw new ArgumentException(nameof(assemblies));

        Assemblies = assemblies;
        AllEventTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();
        UnitOfWorkRelation = AllEventTypes.ToDictionary(type => type, IsSupportUnitOfWork);
    }
}
