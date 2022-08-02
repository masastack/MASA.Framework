// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public class DefaultServiceRegister : IServiceRegister
{
    public void Add(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        var dependency = implementationType.GetCustomAttribute<DependencyAttribute>();
        var descriptor = services.FirstOrDefault(d => d.ServiceType == serviceType);
        if (dependency != null)
        {
            if (descriptor != null)
            {
                var preDependency = descriptor.ImplementationType?.GetCustomAttribute<DependencyAttribute>();
                if (preDependency is { ReplaceServices: true })
                    return;

                if (dependency.ReplaceServices || preDependency is { TryRegister: true })
                    services.Remove(descriptor);
                else if (dependency.TryRegister)
                    return;
            }
        }
        else
        {
            if (descriptor != null)
            {
                var preDependency = descriptor.ImplementationType?.GetCustomAttribute<DependencyAttribute>();
                if (preDependency is { ReplaceServices: true })
                    return;

                if (preDependency is { TryRegister: true })
                    services.Remove(descriptor);
            }
        }

        services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
    }
}
