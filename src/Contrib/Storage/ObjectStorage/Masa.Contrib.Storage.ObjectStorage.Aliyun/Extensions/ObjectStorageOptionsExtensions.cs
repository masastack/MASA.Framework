// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public static class ObjectStorageOptionsExtensions
{
    public static void UseAliyunStorage(
        this ObjectStorageOptions objectStorageOptions,
        string sectionName = Constant.DEFAULT_SECTION)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(sectionName);

        objectStorageOptions.Services.AddConfigure<AliyunStorageConfigureOptions>(sectionName);

        objectStorageOptions.Services.TryAddSingleton<IAliyunStorageOptionProvider>(serviceProvider
            => new DefaultAliyunStorageOptionProvider(serviceProvider
                .GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>()));
        objectStorageOptions.Services.TryAddSingleton<IObjectStorageClientContainer>(serviceProvider
            => new DefaultObjectStorageClientContainer(serviceProvider.GetRequiredService<IObjectStorageClient>(),
                serviceProvider.GetRequiredService<IBucketNameProvider>().GetBucketName()));

        objectStorageOptions.Services.Configure<StorageOptions>(storageOptions =>
        {
            var serviceProvider = objectStorageOptions.Services.BuildServiceProvider();
            var aliyunStorageConfigureOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();

            storageOptions.BucketNames = aliyunStorageConfigureOptionsMonitor.CurrentValue.Storage.BucketNames;

            aliyunStorageConfigureOptionsMonitor.OnChange(options =>
            {
                storageOptions.BucketNames = options.Storage.BucketNames;
            });
        });

        objectStorageOptions.Services.AddAliyunStorageCore();
    }

    public static void UseAliyunStorage(
        this ObjectStorageOptions objectStorageOptions,
        Action<AliyunStorageOptions> action)
    {
        MasaArgumentException.ThrowIfNull(action);

        objectStorageOptions.Services.Configure(action);

        objectStorageOptions.Services.TryAddSingleton<IAliyunStorageOptionProvider>(serviceProvider
            => new DefaultAliyunStorageOptionProvider(
                serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageOptions>>()));

        objectStorageOptions.Services.TryAddSingleton<IObjectStorageClientContainer>(serviceProvider
            => new DefaultObjectStorageClientContainer(serviceProvider.GetRequiredService<IObjectStorageClient>(),
                serviceProvider.GetRequiredService<IBucketNameProvider>().GetBucketName()));

        objectStorageOptions.Services.Configure<StorageOptions>(storageOptions =>
        {
            var serviceProvider = objectStorageOptions.Services.BuildServiceProvider();
            var aliyunStorageOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageOptions>>();

            storageOptions.BucketNames = aliyunStorageOptionsMonitor.CurrentValue.BucketNames;

            aliyunStorageOptionsMonitor.OnChange(options =>
            {
                storageOptions.BucketNames = options.BucketNames;
            });
        });

        objectStorageOptions.Services.AddAliyunStorageCore();
    }

    public static void UseAliyunStorage(
        this ObjectStorageOptions objectStorageOptions,
        AliyunStorageOptions aliyunStorageOptions)
    {
        MasaArgumentException.ThrowIfNull(aliyunStorageOptions);
        objectStorageOptions.UseAliyunStorage(() => aliyunStorageOptions);
    }

    public static void UseAliyunStorage(
        this ObjectStorageOptions objectStorageOptions,
        Func<AliyunStorageOptions> func)
    {
        MasaArgumentException.ThrowIfNull(func);
        objectStorageOptions.UseAliyunStorage(_ => func.Invoke());
    }

    public static void UseAliyunStorage(
        this ObjectStorageOptions objectStorageOptions,
        Func<IServiceProvider, AliyunStorageOptions> func)
    {
        MasaArgumentException.ThrowIfNull(func);

        objectStorageOptions.UseAliyunStorage(aliyunStorageOptions =>
        {
            var options = func.Invoke(objectStorageOptions.Services.BuildServiceProvider());
            aliyunStorageOptions.AccessKeyId = options.AccessKeyId;
            aliyunStorageOptions.AccessKeySecret = options.AccessKeySecret;
            aliyunStorageOptions.Sts = options.Sts;
            aliyunStorageOptions.Endpoint = options.Endpoint;
            aliyunStorageOptions.TemporaryCredentialsCacheKey = options.TemporaryCredentialsCacheKey;
            aliyunStorageOptions.Policy = options.Policy;
            aliyunStorageOptions.RoleArn = options.RoleArn;
            aliyunStorageOptions.RoleSessionName = options.RoleSessionName;
            aliyunStorageOptions.CallbackUrl = options.CallbackUrl;
            aliyunStorageOptions.CallbackBody = options.CallbackBody;
            aliyunStorageOptions.EnableResumableUpload = options.EnableResumableUpload;
            aliyunStorageOptions.BigObjectContentLength = options.BigObjectContentLength;
            aliyunStorageOptions.PartSize = options.PartSize;
            aliyunStorageOptions.BucketNames = options.BucketNames;
        });
    }

    private static void AddAliyunStorageCore(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.TryAddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        services.TryAddSingleton<ICredentialProvider, DefaultCredentialProvider>();
        services.TryAddSingleton<IObjectStorageClient, DefaultStorageClient>();
        services.TryAddSingleton<IBucketNameProvider, BucketNameProvider>();
        services.TryAddSingleton<IObjectStorageClientFactory, DefaultObjectStorageClientFactory>();
    }
}
