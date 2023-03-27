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

        var name = objectStorageBuilder.Name;
        objectStorageBuilder.Services.AddConfigure<AliyunStorageConfigureOptions>(sectionName, name);

        objectStorageBuilder.AddObjectStorage(
            serviceProvider =>
            {
                var aliyunStorageOptions = serviceProvider.GetAliyunStorageOptions(sectionName, name);
                return new DefaultStorageClient(
                    aliyunStorageOptions,
                    null,
                    null,
                    serviceProvider.GetService<IOssClientFactory>(),
                    serviceProvider.GetService<ILoggerFactory>()
                );
            },
            serviceProvider =>
            {
                var aliyunStorageOptions = serviceProvider.GetAliyunStorageOptions(sectionName, name);
                return new BucketNameProvider(aliyunStorageOptions.BucketNames);
            });
    }

    [ExcludeFromCodeCoverage]
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

        var name = objectStorageBuilder.Name;
        objectStorageBuilder.AddObjectStorage(
            serviceProvider =>
            {
                var aliyunStorageOptions = func.Invoke(serviceProvider);
                return new DefaultStorageClient(
                    aliyunStorageOptions,
                    null,
                    null,
                    serviceProvider.GetService<IOssClientFactory>(),
                    serviceProvider.GetService<ILoggerFactory>());

            }, serviceProvider =>
            {
                var aliyunStorageOptions = func.Invoke(serviceProvider);
                return new BucketNameProvider(aliyunStorageOptions.BucketNames);
            });
    }

    private static void AddAliyunStorageCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IOssClientFactory, DefaultOssClientFactory>();
    }
}
