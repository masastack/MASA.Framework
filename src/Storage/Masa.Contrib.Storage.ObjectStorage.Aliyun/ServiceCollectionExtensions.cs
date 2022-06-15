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
    public static IServiceCollection AddAliyunStorage(this IServiceCollection services, string sectionName = Const.DEFAULT_SECTION)
    {
        if (string.IsNullOrEmpty(sectionName))
            throw new ArgumentException(sectionName, nameof(sectionName));

        services.AddAliyunStorageDepend();
        services.TryAddConfigure<AliyunStorageConfigureOptions>(sectionName);
        services.TryAddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        services.TryAddSingleton<ICredentialProvider>(serviceProvider => new DefaultCredentialProvider(
            GetOssClientFactory(serviceProvider),
            GetAliyunStorageConfigurationOption(serviceProvider),
            GetMemoryCache(serviceProvider),
            GetDefaultCredentialProviderLogger(serviceProvider)));
        services.TryAddSingleton<IClient>(serviceProvider => new DefaultStorageClient(
            GetCredentialProvider(serviceProvider),
            GetAliyunStorageOption(serviceProvider),
            GetClientLogger(serviceProvider)));
        return services;
    }

    public static IServiceCollection AddAliyunStorage(this IServiceCollection services, AliyunStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        CheckAliYunStorageOptions(options);

        return services.AddAliyunStorage(() => options);
    }

    public static IServiceCollection AddAliyunStorage(this IServiceCollection services, Func<IServiceProvider, AliyunStorageOptions> func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        services.AddAliyunStorageDepend();
        services.TryAddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        services.TryAddSingleton<ICredentialProvider>(serviceProvider => new DefaultCredentialProvider(
            GetOssClientFactory(serviceProvider),
            func.Invoke(serviceProvider),
            GetMemoryCache(serviceProvider),
            GetDefaultCredentialProviderLogger(serviceProvider)));
        services.TryAddSingleton<IClient>(serviceProvider => new DefaultStorageClient(
            GetCredentialProvider(serviceProvider),
            func.Invoke(serviceProvider),
            GetClientLogger(serviceProvider)));
        return services;
    }

    public static IServiceCollection AddAliyunStorage(this IServiceCollection services, Func<AliyunStorageOptions> func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        return services.AddAliyunStorage(_ => func.Invoke());
    }

    private static IServiceCollection AddAliyunStorageDepend(this IServiceCollection services)
    {
        services.AddMemoryCache();
        return services;
    }

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        var serviceProvider = services.BuildServiceProvider();
        IConfiguration? configuration = serviceProvider.GetService<IMasaConfiguration>()?.GetConfiguration(SectionTypes.Local) ??
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

    private static IOssClientFactory GetOssClientFactory(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IOssClientFactory>();

    private static ICredentialProvider GetCredentialProvider(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<ICredentialProvider>();

    private static IOptionsMonitor<AliyunStorageConfigureOptions> GetAliyunStorageConfigurationOption(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();

    private static IOptionsMonitor<AliyunStorageOptions> GetAliyunStorageOption(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageOptions>>();

    private static IMemoryCache GetMemoryCache(IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<IMemoryCache>();

    private static ILogger<DefaultStorageClient>? GetClientLogger(IServiceProvider serviceProvider) => serviceProvider.GetService<ILogger<DefaultStorageClient>>();

    private static ILogger<DefaultCredentialProvider>? GetDefaultCredentialProviderLogger(IServiceProvider serviceProvider)
        => serviceProvider.GetService<ILogger<DefaultCredentialProvider>>();

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
