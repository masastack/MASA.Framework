// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public class DefaultTypeProvider : TypeProviderBase
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
            return types.Where(t => t.IsClass && IsAssignableFrom(serviceType, t) && !IsSkip(t)).ToList();

        return new List<Type>
        {
            serviceType
        };
    }

    public virtual List<Type> GetServiceTypes(List<Type> types, Type interfaceType)
    {
        var interfaceServiceTypes = types.Where(t => t.IsInterface && t != interfaceType && interfaceType.IsAssignableFrom(t));
        var classServiceTypes = types.Where(type
            => IsAssignableFrom(interfaceType, type) &&
            !interfaceServiceTypes.Any(t => IsAssignableFrom(t, type)) &&
            !IsSkip(type));

        var list = new List<Type>(interfaceServiceTypes).Concat(classServiceTypes);
        return list.ToList();
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
