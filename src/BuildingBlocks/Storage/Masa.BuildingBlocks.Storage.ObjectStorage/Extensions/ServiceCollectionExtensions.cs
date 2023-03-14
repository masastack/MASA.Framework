// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObjectStorage(
        this IServiceCollection services,
        Action<ObjectStorageBuilder> configure)
        => services.AddObjectStorage(Options.Options.DefaultName, configure);

    public static IServiceCollection AddObjectStorage(
        this IServiceCollection services,
        string name,
        Action<ObjectStorageBuilder> configure)
    {
        services.AddObjectStorageCore(name);
        var storageOptions = new ObjectStorageBuilder(services, name);
        configure.Invoke(storageOptions);
        return services;
    }

    private static void AddObjectStorageCore(this IServiceCollection services, string name)
    {
        MasaArgumentException.ThrowIfNull(services);
        MasaArgumentException.ThrowIfNull(name);

        services.TryAddSingleton<IObjectStorageClientFactory, DefaultObjectStorageClientFactory>();
        services.TryAddObjectStorageClient();
        services.TryAddBucketNameProvider();
        services.TryAddObjectStorageClientContainer();

        services.AddServiceFactory();
        MasaApp.TrySetServiceCollection(services);
    }

    private static void TryAddObjectStorageClient(this IServiceCollection services)
    {
        services.TryAddTransient<IObjectStorageClient>(serviceProvider =>
            serviceProvider.GetRequiredService<IObjectStorageClientFactory>().Create());
        services.TryAddTransient<IObjectStorageClientFactory, DefaultObjectStorageClientFactory>();
    }

    private static void TryAddBucketNameProvider(this IServiceCollection services)
    {
        services.TryAddTransient<IBucketNameProvider>(serviceProvider =>
            serviceProvider.GetRequiredService<IBucketNameFactory>().Create());
        services.TryAddTransient<IBucketNameFactory, DefaultBucketNameFactory>();
    }

    private static void TryAddObjectStorageClientContainer(this IServiceCollection services)
    {
        services.TryAddTransient<IObjectStorageClientContainer>(serviceProvider
            => new DefaultObjectStorageClientContainer(
                serviceProvider.GetRequiredService<IObjectStorageClient>(),
                serviceProvider.GetRequiredService<IBucketNameProvider>().GetBucketName()));

        services.TryAddTransient(typeof(IObjectStorageClientContainer<>), typeof(DefaultObjectStorageClientContainer<>));
        services.TryAddTransient<IObjectStorageClientContainerFactory, DefaultObjectStorageClientContainerFactory>();
    }
}
