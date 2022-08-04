// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public class DefaultTypeProvider : BaseTypeProvider
{
    public override List<ServiceDescriptorOptions> GetServiceDescriptors(List<Type> types)
        => GetServiceDescriptorCore(types, typeof(ISingletonDependency), ServiceLifetime.Singleton)
            .Concat(GetServiceDescriptorCore(types, typeof(IScopedDependency), ServiceLifetime.Scoped))
            .Concat(GetServiceDescriptorCore(types, typeof(ITransientDependency), ServiceLifetime.Transient)).ToList();

    public virtual List<ServiceDescriptorOptions> GetServiceDescriptorCore(List<Type> types, Type type, ServiceLifetime lifetime)
    {
        List<ServiceDescriptorOptions> descriptors = new();
        var serviceTypes = GetServiceTypes(types, type);
        foreach (var serviceType in serviceTypes)
        {
            var implementationTypes = GetImplementationTypes(types, serviceType);
            foreach (var implementationType in implementationTypes)
            {
                if (serviceType.IsGenericType &&
                    implementationType.IsGenericType &&
                    serviceType.GetTypeInfo().GenericTypeParameters.Length != implementationType.GetTypeInfo().GenericTypeParameters.Length)
                    continue;

                descriptors.Add(new ServiceDescriptorOptions(serviceType, implementationType, lifetime, AutoFire(serviceType)));
            }
        }

        return descriptors;
    }

    public virtual bool AutoFire(Type serviceType)
        => IsAssignableFrom(typeof(IAutoFireDependency), serviceType);

    public virtual List<Type> GetImplementationTypes(List<Type> types, Type serviceType)
    {
        if (serviceType.IsInterface)
            return types.Where(t => !t.IsAbstract && t.IsClass && IsAssignableFrom(serviceType, t)).ToList();

        return new List<Type>
        {
            serviceType
        };
    }

    public virtual List<Type> GetServiceTypes(List<Type> types, Type interfaceType)
    {
        var interfaceServiceTypes = types.Where(t => t.IsInterface && t != interfaceType && interfaceType.IsAssignableFrom(t));
        return types.Where(type
                => IsAssignableFrom(interfaceType, type) && !type.GetInterfaces().Any(t => interfaceServiceTypes.Contains(t)) &&
                !IsSkip(type))
            .Concat(interfaceServiceTypes)
            .ToList();
    }

    public virtual bool IsSkip(Type type)
    {
        if (type.IsAbstract)
            return true;

        var ignoreInjection = type.GetCustomAttribute<IgnoreInjectionAttribute>();
        if (ignoreInjection == null)
            return false;

        var inheritIgnoreInjection = type.GetCustomAttribute<IgnoreInjectionAttribute>(false);
        if (inheritIgnoreInjection != null)
            return true;

        return ignoreInjection.Cascade;
    }
}
