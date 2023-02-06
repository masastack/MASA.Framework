// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObjectStorage(
        this IServiceCollection services,
        Action<ObjectStorageOptions> action)
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddSingleton<IObjectStorageClientFactory, DefaultObjectStorageClientFactory>();
        services.TryAddTransient(serviceProvider => serviceProvider.GetRequiredService<IObjectStorageClientFactory>().Create());
        services.TryAddSingleton(typeof(IObjectStorageClientContainer<>), typeof(DefaultObjectStorageClientContainer<>));

        var storageOptions = new ObjectStorageOptions(services);
        action.Invoke(storageOptions);
        return services;
    }
}
