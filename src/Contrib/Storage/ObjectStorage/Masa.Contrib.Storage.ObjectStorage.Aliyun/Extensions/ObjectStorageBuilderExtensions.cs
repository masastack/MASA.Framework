// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public static class ObjectStorageBuilderExtensions
{
    public static void UseAliyunStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        string sectionName = Constant.DEFAULT_SECTION)
    {
        objectStorageBuilder.Services.AddAliyunStorageCore();

        objectStorageBuilder.Services.AddConfigure<AliyunStorageConfigureOptions>(sectionName, objectStorageBuilder.Name);

        objectStorageBuilder.AddObjectStorage(
            objectStorageBuilder.Name,
            serviceProvider =>
            {
                var ossClientFactory = serviceProvider.GetRequiredService<IOssClientFactory>();
                var aliyunStorageOptionProvider = serviceProvider.GetAliyunStorageOptionProvider(sectionName, objectStorageBuilder.Name);

                var credentialProvider = new DefaultCredentialProvider(
                    ossClientFactory,
                    aliyunStorageOptionProvider,
                    new MemoryCache(Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions())),
                    serviceProvider.GetService<ILogger<DefaultCredentialProvider>>()
                );
                return new DefaultStorageClient(
                    credentialProvider,
                    aliyunStorageOptionProvider,
                    serviceProvider.GetService<ILogger<DefaultStorageClient>>());
            },
            serviceProvider =>
            {
                if (serviceProvider.IsEnabledIsolation(
                        sectionName,
                        objectStorageBuilder.Name,
                        out AliyunStorageConfigureOptions? aliyunStorageConfigureOptions,
                        out IOptionsMonitor<AliyunStorageConfigureOptions>? defaultStorageConfigureOptionsMonitor))
                {
                    return new BucketNameProvider(aliyunStorageConfigureOptions.Storage.BucketNames);
                }

                var bucketNameProvider =
                    new BucketNameProvider(defaultStorageConfigureOptionsMonitor.Get(objectStorageBuilder.Name).Storage.BucketNames);
                defaultStorageConfigureOptionsMonitor.OnChange(options =>
                {
                    bucketNameProvider.BucketNames = options.Storage.BucketNames;
                });
                return bucketNameProvider;
            });
    }

    public static void UseAliyunStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        Action<AliyunStorageOptions> configure)
    {
        MasaArgumentException.ThrowIfNull(configure);

        objectStorageBuilder.UseAliyunStorage(_ =>
        {
            var aliyunStorageOptions = new AliyunStorageOptions();
            configure.Invoke(aliyunStorageOptions);
            return aliyunStorageOptions;
        });
    }

    public static void UseAliyunStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        AliyunStorageOptions aliyunStorageOptions)
    {
        MasaArgumentException.ThrowIfNull(aliyunStorageOptions);

        objectStorageBuilder.UseAliyunStorage(_ => aliyunStorageOptions);
    }

    public static void UseAliyunStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        Func<AliyunStorageOptions> func)
    {
        MasaArgumentException.ThrowIfNull(func);
        objectStorageBuilder.UseAliyunStorage(_ => func.Invoke());
    }

    public static void UseAliyunStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        Func<IServiceProvider, AliyunStorageOptions> func)
    {
        MasaArgumentException.ThrowIfNull(func);
        objectStorageBuilder.Services.AddAliyunStorageCore();

        objectStorageBuilder.AddObjectStorage(
            objectStorageBuilder.Name,
            serviceProvider =>
            {
                var defaultAliyunStorageOptionProvider = serviceProvider.GetRequiredService<IAliyunStorageOptionProvider>();
                var credentialProvider = new DefaultCredentialProvider(
                    serviceProvider.GetRequiredService<IOssClientFactory>(),
                    defaultAliyunStorageOptionProvider,
                    new MemoryCache(Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions())),
                    serviceProvider.GetService<ILogger<DefaultCredentialProvider>>()
                );
                return new DefaultStorageClient(credentialProvider,
                    defaultAliyunStorageOptionProvider,
                    serviceProvider.GetService<ILogger<DefaultStorageClient>>());
            }, serviceProvider =>
            {
                var aliyunStorageOptions = func.Invoke(serviceProvider);
                return new BucketNameProvider(aliyunStorageOptions.BucketNames);
            });
        objectStorageBuilder.Services.TryAddTransient<IAliyunStorageOptionProvider>(serviceProvider
            => new DefaultAliyunStorageOptionProvider(func.Invoke(serviceProvider)));
    }

    private static void AddAliyunStorageCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IOssClientFactory, DefaultOssClientFactory>();
    }
}
