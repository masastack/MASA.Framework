// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedLock(this IServiceCollection services, Action<MedallionBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (services.Any<IMasaDistributedLock>())
            services.RemoveAll<IMasaDistributedLock>();

        builder.Invoke(new MedallionBuilder(services));

        if (!services.Any<IDistributedLockProvider>())
            throw new MasaException("Please add IDistributedLockProvider first.");

        services.TryAddSingleton<IMasaDistributedLock, DefaultMedallionDistributedLock>();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
