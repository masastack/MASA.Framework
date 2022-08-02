// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection.Options;

public class ServiceDescriptorOptions
{
    public Type ServiceType { get; }

    public Type ImplementationType { get; }

    public ServiceLifetime Lifetime { get; }

    public bool AutoFire { get; }

    public ServiceDescriptorOptions(Type serviceType, Type implementationType, ServiceLifetime lifetime, bool autoFire)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
        AutoFire = autoFire;
    }
}
