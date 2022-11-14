// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Alibaba Cloud Storage
    /// Load configuration information according to the specified SectionName node
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName">node name, defaults to Aliyun</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IServiceCollection AddAliyunStorage(
        this IServiceCollection services,
        string sectionName = Constant.DEFAULT_SECTION)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(sectionName);

        services.AddConfigure<StorageOptions>($"{sectionName}{ConfigurationPath.KeyDelimiter}{nameof(AliyunStorageConfigureOptions.Storage)}");
        services.AddConfigure<AliyunStorageConfigureOptions>(sectionName);
        services.TryAddSingleton<IAliyunStorageOptionProvider>(serviceProvider
            => new DefaultAliyunStorageOptionProvider(GetAliyunStorageConfigurationOption(serviceProvider)));
        services.TryAddSingleton<IClientContainer>(serviceProvider
            => new DefaultClientContainer(serviceProvider.GetRequiredService<IClient>(),
                serviceProvider.GetRequiredService<IBucketNameProvider>().GetBucketName()));
        return services.AddAliyunStorageCore();
    }

    public static IServiceCollection AddAliyunStorage(
        this IServiceCollection services,
        AliyunStorageOptions options,
        string? defaultBucketName = null)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        CheckAliYunStorageOptions(options);

        return services.AddAliyunStorage(() => options, defaultBucketName);
    }

    public static IServiceCollection AddAliyunStorage(
        this IServiceCollection services,
        Func<IServiceProvider, AliyunStorageOptions> func,
        string? defaultBucketName = null)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        services.TryAddSingleton<IAliyunStorageOptionProvider>(serviceProvider
            => new DefaultAliyunStorageOptionProvider(func.Invoke(serviceProvider)));
        return services.AddAliyunStorageCore(defaultBucketName);
    }

    public static IServiceCollection AddAliyunStorage(
        this IServiceCollection services,
        Func<IServiceProvider, Task<AliyunStorageOptions>> func,
        string? defaultBucketName = null)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        services.TryAddSingleton<IAliyunStorageOptionProvider>(serviceProvider
            => new DefaultAliyunStorageOptionProvider(func.Invoke(serviceProvider).ConfigureAwait(false).GetAwaiter().GetResult()));
        return services.AddAliyunStorageCore(defaultBucketName);
    }

    public static IServiceCollection AddAliyunStorage(
        this IServiceCollection services,
        Func<AliyunStorageOptions> func,
        string? defaultBucketName = null)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        return services.AddAliyunStorage(_ => func.Invoke(), defaultBucketName);
    }

    private static IServiceCollection AddAliyunStorageCore(this IServiceCollection services, string? defaultBucketName = null)
    {
        services.AddMemoryCache();
        services.TryAddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        services.TryAddSingleton<ICredentialProvider, DefaultCredentialProvider>();
        services.TryAddSingleton<IClient, DefaultStorageClient>();
        if (defaultBucketName != null)
        {
            services.Configure<StorageOptions>(option =>
            {
                option.BucketNames = new BucketNames(new List<KeyValuePair<string, string>>()
                {
                    new(BucketNames.DEFAULT_BUCKET_NAME, defaultBucketName)
                });
            });
            services.TryAddSingleton<IClientContainer>(serviceProvider
                => new DefaultClientContainer(serviceProvider.GetRequiredService<IClient>(), defaultBucketName));
        }


        services.TryAddSingleton<IBucketNameProvider, BucketNameProvider>();
        services.TryAddSingleton(typeof(IClientContainer<>), typeof(DefaultClientContainer<>));
        services.TryAddSingleton<IClientFactory, DefaultClientFactory>();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    private static IOptionsMonitor<AliyunStorageConfigureOptions> GetAliyunStorageConfigurationOption(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();

    private static void CheckAliYunStorageOptions(AliyunStorageOptions options)
    {
        MasaArgumentException.ThrowIfNull(options);

        ObjectStorageExtensions.CheckNullOrEmptyAndReturnValue(options.AccessKeyId, nameof(options.AccessKeyId));
        ObjectStorageExtensions.CheckNullOrEmptyAndReturnValue(options.AccessKeySecret, nameof(options.AccessKeySecret));
    }
}
