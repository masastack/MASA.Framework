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

    public static IServiceCollection AddObjectStorage(
        this IServiceCollection services,
        string name,
        Func<IServiceProvider, IObjectStorageClient> implementationFactory,
        Func<IServiceProvider, IBucketNameProvider> bucketNameImplementationFactory)
    {
        services.AddObjectStorageCore(name);

        services.Configure<ObjectStorageFactoryOptions>(callerOptions =>
        {
            if (callerOptions.Options.Any(relation => relation.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The ObjectStorage name already exists, please change the name, the repeat name is [{name}]");

            callerOptions.Options.Add(new(name, implementationFactory));
        });

        services.Configure<BucketNameFactoryOptions>(callerOptions =>
        {
            if (callerOptions.Options.Any(relation => relation.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The Bucket name already exists, please change the name, the repeat name is [{name}]");

            callerOptions.Options.Add(new(name, bucketNameImplementationFactory));
        });

        MasaApp.TrySetServiceCollection(services);
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

        MasaApp.TrySetServiceCollection(services);
    }

    private static void TryAddObjectStorageClient(this IServiceCollection services)
    {
        services.TryAddSingleton<IObjectStorageClientSingleton>(serviceProvider
            => (IObjectStorageClientSingleton)serviceProvider.GetRequiredService<IObjectStorageClientFactory>().Create());
        services.TryAddScoped<IObjectStorageClientScoped>(serviceProvider
            => (IObjectStorageClientScoped)serviceProvider.GetRequiredService<IObjectStorageClientFactory>().Create());
        services.TryAddTransient<IObjectStorageClientTransient>(serviceProvider
            => (IObjectStorageClientTransient)serviceProvider.GetRequiredService<IObjectStorageClientFactory>().Create());
        services.TryAddTransient<IObjectStorageClient>(serviceProvider =>
        {
            var lifetime = serviceProvider.GetService<IOptions<ObjectStorageClientLifetimeOptions>>()?.Value.Lifetime ??
                serviceProvider.GetService<IOptions<GlobalClientLifetimeOptions>>()?.Value.Lifetime;
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                case null when IsolationConfiguration.IsEnable:
                    return serviceProvider.GetRequiredService<IObjectStorageClientScoped>();
                case ServiceLifetime.Transient:
                    return serviceProvider.GetRequiredService<IObjectStorageClientTransient>();
                default:
                    return serviceProvider.GetRequiredService<IObjectStorageClientSingleton>();
            }
        });
        services.TryAddSingleton<IObjectStorageClientFactory, DefaultObjectStorageClientFactory>();
    }

    private static void TryAddBucketNameProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<IBucketNameProviderSingleton>(serviceProvider
            => (IBucketNameProviderSingleton)serviceProvider.GetRequiredService<IBucketNameFactory>().Create());
        services.TryAddScoped<IBucketNameProviderScoped>(serviceProvider
            => (IBucketNameProviderScoped)serviceProvider.GetRequiredService<IBucketNameFactory>().Create());
        services.TryAddTransient<IBucketNameProviderTransient>(serviceProvider
            => (IBucketNameProviderTransient)serviceProvider.GetRequiredService<IBucketNameFactory>().Create());
        services.TryAddTransient<IBucketNameProvider>(serviceProvider =>
        {
            var lifetime = serviceProvider.GetService<IOptions<ObjectStorageClientLifetimeOptions>>()?.Value.Lifetime ??
                serviceProvider.GetService<IOptions<GlobalClientLifetimeOptions>>()?.Value.Lifetime;
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                case null when IsolationConfiguration.IsEnable:
                    return serviceProvider.GetRequiredService<IBucketNameProviderScoped>();
                case ServiceLifetime.Transient:
                    return serviceProvider.GetRequiredService<IBucketNameProviderTransient>();
                default:
                    return serviceProvider.GetRequiredService<IBucketNameProviderSingleton>();
            }
        });
        services.TryAddSingleton<IBucketNameFactory, DefaultBucketNameFactory>();
    }

    private static void TryAddObjectStorageClientContainer(this IServiceCollection services)
    {
        services.TryAddTransient<IObjectStorageClientContainer>(serviceProvider
            => new DefaultObjectStorageClientContainer(
                serviceProvider.GetRequiredService<IObjectStorageClient>(),
                serviceProvider.GetRequiredService<IBucketNameProvider>().GetBucketName()));

        services.TryAddTransient(typeof(IObjectStorageClientContainer<>), typeof(DefaultObjectStorageClientContainer<>));
        services.TryAddSingleton<IObjectStorageClientContainerFactory, DefaultObjectStorageClientContainerFactory>();
    }
}
