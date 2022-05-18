// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EF.Internal;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddEnumerable(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceDescriptor serviceDescriptor)
    {
        if (services.Any(s => s.ServiceType == serviceType && s.ImplementationType == implementationType))
        {
            return services;
        }

        services.Add(serviceDescriptor);
        return services;
    }
}
