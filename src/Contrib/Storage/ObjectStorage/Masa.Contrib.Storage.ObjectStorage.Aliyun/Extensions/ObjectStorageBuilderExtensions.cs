// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public static class ObjectStorageBuilderExtensions
{
    // public static void UseAliyunStorage(
    //     this ObjectStorageBuilder objectStorageBuilder,
    //     string sectionName = Constant.DEFAULT_SECTION)
    // {
    //     objectStorageBuilder.Services.AddConfigure<AliyunStorageConfigureOptions>(sectionName, objectStorageBuilder.Name);
    //
    //     objectStorageBuilder.Services.TryAddSingleton<IAliyunStorageOptionProvider>(serviceProvider
    //         => new DefaultAliyunStorageOptionProvider(serviceProvider
    //             .GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>(), objectStorageBuilder.Name));
    //     objectStorageBuilder.Services.TryAddSingleton<IObjectStorageClientContainer>(serviceProvider
    //         => new DefaultObjectStorageClientContainer(serviceProvider.GetRequiredService<IObjectStorageClient>(),
    //             serviceProvider.GetRequiredService<IBucketNameProvider>().GetBucketName()));
    //
    //     objectStorageBuilder.Services.Configure<StorageOptions>(storageOptions =>
    //     {
    //         var serviceProvider = objectStorageBuilder.Services.BuildServiceProvider();
    //         var aliyunStorageConfigureOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();
    //
    //         storageOptions.BucketNames = aliyunStorageConfigureOptionsMonitor.CurrentValue.Storage.BucketNames;
    //
    //         aliyunStorageConfigureOptionsMonitor.OnChange(options =>
    //         {
    //             storageOptions.BucketNames = options.Storage.BucketNames;
    //         });
    //     });
    //
    //     objectStorageBuilder.Services.AddAliyunStorageCore();
    // }
    //
    // public static void UseAliyunStorage(
    //     this ObjectStorageBuilder objectStorageBuilder,
    //     Action<AliyunStorageOptions> action)
    // {
    //     MasaArgumentException.ThrowIfNull(action);
    //
    //     objectStorageBuilder.Services.Configure(action);
    //
    //     objectStorageBuilder.Services.TryAddSingleton<IAliyunStorageOptionProvider>(serviceProvider
    //         => new DefaultAliyunStorageOptionProvider(
    //             serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageOptions>>()));
    //
    //     objectStorageBuilder.Services.TryAddSingleton<IObjectStorageClientContainer>(serviceProvider
    //         => new DefaultObjectStorageClientContainer(serviceProvider.GetRequiredService<IObjectStorageClient>(),
    //             serviceProvider.GetRequiredService<IBucketNameProvider>().GetBucketName()));
    //
    //     objectStorageBuilder.Services.Configure<StorageOptions>(storageOptions =>
    //     {
    //         var serviceProvider = objectStorageBuilder.Services.BuildServiceProvider();
    //         var aliyunStorageOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageOptions>>();
    //
    //         storageOptions.BucketNames = aliyunStorageOptionsMonitor.CurrentValue.BucketNames;
    //
    //         aliyunStorageOptionsMonitor.OnChange(options =>
    //         {
    //             storageOptions.BucketNames = options.BucketNames;
    //         });
    //     });
    //
    //     objectStorageBuilder.Services.AddAliyunStorageCore();
    // }
    //
    // public static void UseAliyunStorage(
    //     this ObjectStorageBuilder objectStorageBuilder,
    //     AliyunStorageOptions aliyunStorageOptions)
    // {
    //     MasaArgumentException.ThrowIfNull(aliyunStorageOptions);
    //     objectStorageBuilder.UseAliyunStorage(() => aliyunStorageOptions);
    // }
    //
    // public static void UseAliyunStorage(
    //     this ObjectStorageBuilder objectStorageBuilder,
    //     Func<AliyunStorageOptions> func)
    // {
    //     MasaArgumentException.ThrowIfNull(func);
    //     objectStorageBuilder.UseAliyunStorage(_ => func.Invoke());
    // }
    //
    // public static void UseAliyunStorage(
    //     this ObjectStorageBuilder objectStorageBuilder,
    //     Func<IServiceProvider, AliyunStorageOptions> func)
    // {
    //     MasaArgumentException.ThrowIfNull(func);
    //
    //     objectStorageBuilder.UseAliyunStorage(aliyunStorageOptions =>
    //     {
    //         var options = func.Invoke(objectStorageBuilder.Services.BuildServiceProvider());
    //         aliyunStorageOptions.AccessKeyId = options.AccessKeyId;
    //         aliyunStorageOptions.AccessKeySecret = options.AccessKeySecret;
    //         aliyunStorageOptions.Sts = options.Sts;
    //         aliyunStorageOptions.Endpoint = options.Endpoint;
    //         aliyunStorageOptions.TemporaryCredentialsCacheKey = options.TemporaryCredentialsCacheKey;
    //         aliyunStorageOptions.Policy = options.Policy;
    //         aliyunStorageOptions.RoleArn = options.RoleArn;
    //         aliyunStorageOptions.RoleSessionName = options.RoleSessionName;
    //         aliyunStorageOptions.CallbackUrl = options.CallbackUrl;
    //         aliyunStorageOptions.CallbackBody = options.CallbackBody;
    //         aliyunStorageOptions.EnableResumableUpload = options.EnableResumableUpload;
    //         aliyunStorageOptions.BigObjectContentLength = options.BigObjectContentLength;
    //         aliyunStorageOptions.PartSize = options.PartSize;
    //         aliyunStorageOptions.BucketNames = options.BucketNames;
    //     });
    // }

    public static void UseAliyunStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        AliyunStorageOptions aliyunStorageOptions)
    {
        MasaArgumentException.ThrowIfNull(aliyunStorageOptions);

        objectStorageBuilder.UseAliyunStorage(_ => aliyunStorageOptions);
    }

    public static void UseAliyunStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        string sectionName = Constant.DEFAULT_SECTION)
    {
        objectStorageBuilder.Services.AddAliyunStorageCore();

        objectStorageBuilder.Services.AddConfigure<AliyunStorageConfigureOptions>(sectionName, objectStorageBuilder.Name);

        objectStorageBuilder.Services.TryAddTransient<IAliyunStorageOptionProvider>(serviceProvider
            =>
        {
            if (IsEnabledIsolation(serviceProvider,
                    out AliyunStorageConfigureOptions? aliyunStorageConfigureOptions,
                    out IOptionsMonitor<AliyunStorageConfigureOptions>? optionsMonitor))
            {
                return new DefaultAliyunStorageOptionProvider(aliyunStorageConfigureOptions);
            }
            return
                new DefaultAliyunStorageOptionProvider(optionsMonitor, objectStorageBuilder.Name);
        });

        objectStorageBuilder.Services.AddObjectStorage(
            objectStorageBuilder.Name,
            serviceProvider =>
            {
                var ossClientFactory = serviceProvider.GetRequiredService<IOssClientFactory>();
                var aliyunStorageOptionProvider = serviceProvider.GetRequiredService<IAliyunStorageOptionProvider>();

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
                if (IsEnabledIsolation(serviceProvider,
                        out AliyunStorageConfigureOptions? aliyunStorageConfigureOptions,
                        out IOptionsMonitor<AliyunStorageConfigureOptions>? optionsMonitor))
                {
                    return new BucketNameProvider(aliyunStorageConfigureOptions.Storage.BucketNames);
                }

                var bucketNameProvider = new BucketNameProvider(optionsMonitor.Get(objectStorageBuilder.Name).Storage.BucketNames);
                optionsMonitor.OnChange(options =>
                {
                    bucketNameProvider.BucketNames = options.Storage.BucketNames;
                });
                return bucketNameProvider;
            });

        AliyunStorageConfigureOptions GetDefaultAliyunStorageConfigureOptions(IServiceProvider serviceProvider)
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();
            return optionsMonitor.Get(objectStorageBuilder.Name);
        }

        bool IsEnabledIsolation(
            IServiceProvider serviceProvider,
            [NotNullWhen(true)] out AliyunStorageConfigureOptions? aliyunStorageConfigureOptions,
            [NotNullWhen(false)] out IOptionsMonitor<AliyunStorageConfigureOptions>? optionsMonitor)
        {
            var isEnable = IsolationConfiguration.IsEnable;
            if (isEnable)
            {
                var isolationConfigurationProvider = serviceProvider.GetRequiredService<IIsolationConfigurationProvider>();
                if (!isolationConfigurationProvider.TryGetModule(sectionName, out aliyunStorageConfigureOptions))
                {
                    //使用默认配置,增加日志
                    aliyunStorageConfigureOptions = GetDefaultAliyunStorageConfigureOptions(serviceProvider);
                }
                optionsMonitor = null;
            }
            else
            {
                aliyunStorageConfigureOptions = null;
                optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();
            }
            return isEnable;
        }
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

        objectStorageBuilder.Services.AddObjectStorage(
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
                return new DefaultStorageClient(credentialProvider, defaultAliyunStorageOptionProvider,
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
