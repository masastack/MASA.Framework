// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
        string sectionName = Const.DEFAULT_SECTION)
    {
        if (string.IsNullOrEmpty(sectionName))
            throw new ArgumentException(sectionName, nameof(sectionName));

        services.TryAddConfigure<StorageOptions>($"{sectionName}{ConfigurationPath.KeyDelimiter}{nameof(AliyunStorageConfigureOptions.Storage)}");
        services.TryAddConfigure<AliyunStorageConfigureOptions>(sectionName);
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
        return services;
    }

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        var serviceProvider = services.BuildServiceProvider();
        IConfiguration? configuration = serviceProvider.GetService<IMasaConfiguration>()?.Local ??
            serviceProvider.GetService<IConfiguration>();

        if (configuration == null)
            return services;

        string name = Microsoft.Extensions.Options.Options.DefaultName;
        services.AddOptions();
        var configurationSection = configuration.GetSection(sectionName);
        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, configurationSection));
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name,
            configurationSection, _ =>
            {
            }));
        return services;
    }

    private static IOptionsMonitor<AliyunStorageConfigureOptions> GetAliyunStorageConfigurationOption(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();

    private static IAliyunStorageOptionProvider GetAliyunStorageOptionProvider(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IAliyunStorageOptionProvider>();


    private static void CheckAliYunStorageOptions(AliyunStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        if (options.AccessKeyId == null && options.AccessKeySecret == null)
            throw new ArgumentException(
                $"{nameof(options.AccessKeyId)}, {nameof(options.AccessKeySecret)} are required and cannot be empty");

        ObjectStorageExtensions.CheckNullOrEmptyAndReturnValue(options.AccessKeyId, nameof(options.AccessKeyId));
        ObjectStorageExtensions.CheckNullOrEmptyAndReturnValue(options.AccessKeySecret, nameof(options.AccessKeySecret));
    }
}
