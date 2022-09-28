// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocalDistributedLock(this IServiceCollection services)
    {
        if (services.Any<IDistributedLock>())
            return services;

        services.TryAddSingleton<IDistributedLock, DefaultLocalDistributedLock>();

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
