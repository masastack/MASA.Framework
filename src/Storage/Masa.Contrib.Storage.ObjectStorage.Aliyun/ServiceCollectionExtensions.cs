namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

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

        return services.AddAliyunStorageCore(() =>
        {
            services.TryAddConfigure<AliyunStorageOptions>(sectionName);
            services.AddSingleton<IClient>(serviceProvider =>
            {
                var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageOptions>>();
                CheckAliYunStorageOptions(optionsMonitor.CurrentValue, $"Failed to get {nameof(IOptionsMonitor<AliyunStorageOptions>)}");
                return new Client(optionsMonitor.CurrentValue, GetMemoryCache(serviceProvider), GetClientLogger(serviceProvider));
            });
        });
    }

    public static IServiceCollection AddAliyunStorage(this IServiceCollection services, AliyunStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        string message = $"{options.AccessKeyId}, {options.AccessKeySecret}, {options.RegionId}, {options.RoleArn}, {options.RoleSessionName} are required and cannot be empty";
        CheckAliYunStorageOptions(options, message);

        return services.AddAliyunStorage(() => options);
    }

    public static IServiceCollection AddAliyunStorage(this IServiceCollection services, Func<AliyunStorageOptions> func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        return services.AddAliyunStorageCore(() => services.AddSingleton<IClient>(serviceProvider
            => new Client(func.Invoke(), GetMemoryCache(serviceProvider), GetClientLogger(serviceProvider))));
    }

    private static IServiceCollection AddAliyunStorageCore(this IServiceCollection services, Action action)
    {
        if (services.Any(service => service.ImplementationType == typeof(AliyunStorageProvider)))
            return services;

        services.AddSingleton<AliyunStorageProvider>();
        services.AddMemoryCache();
        action.Invoke();
        return services;
    }

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        IConfiguration?
            configuration =
                services.BuildServiceProvider().GetService<IConfiguration>(); //Todo: Follow-up needs to support IMasaConfiguration
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

    private static IMemoryCache GetMemoryCache(IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<IMemoryCache>();

    private static ILogger<Client>? GetClientLogger(IServiceProvider serviceProvider) => serviceProvider.GetService<ILogger<Client>>();

    private static void CheckAliYunStorageOptions(AliyunStorageOptions options, string? message = default)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        if (options.AccessKeyId == null &&
            options.AccessKeySecret == null &&
            options.RegionId == null &&
            options.RoleArn == null &&
            options.RoleSessionName == null)
            throw new ArgumentException(message);

        options.CheckNullOrEmptyAndReturnValue(options.AccessKeyId, nameof(options.AccessKeyId));
        options.CheckNullOrEmptyAndReturnValue(options.AccessKeySecret, nameof(options.AccessKeySecret));
        options.CheckNullOrEmptyAndReturnValue(options.RegionId, nameof(options.RegionId));
        options.CheckNullOrEmptyAndReturnValue(options.RoleArn, nameof(options.RoleArn));
        options.CheckNullOrEmptyAndReturnValue(options.RoleSessionName, nameof(options.RoleSessionName));
    }

    private class AliyunStorageProvider
    {
    }
}
