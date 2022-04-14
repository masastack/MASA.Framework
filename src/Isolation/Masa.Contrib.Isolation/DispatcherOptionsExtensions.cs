namespace Masa.Contrib.Isolation;

public static class DispatcherOptionsExtensions
{
    /// <summary>
    /// It is not recommended to use directly here, please use UseIsolationUoW
    /// </summary>
    /// <param name="eventBusBuilder"></param>
    /// <param name="isolationBuilder"></param>
    /// <returns></returns>
    public static IEventBusBuilder UseIsolation(this IEventBusBuilder eventBusBuilder, Action<IsolationBuilder> isolationBuilder)
    {
        eventBusBuilder.Services.AddIsolation(isolationBuilder);
        return eventBusBuilder;
    }

    /// <summary>
    /// It is not recommended to use directly here, please use UseIsolationUoW
    /// </summary>
    /// <param name="options"></param>
    /// <param name="isolationBuilder"></param>
    /// <returns></returns>
    public static IDispatcherOptions UseIsolation(this IDispatcherOptions options, Action<IsolationBuilder> isolationBuilder)
    {
        options.Services.AddIsolation(isolationBuilder);
        return options;
    }

    private static void AddIsolation(this IServiceCollection services, Action<IsolationBuilder> isolationBuilder)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(isolationBuilder);

        if (services.Any(service => service.ImplementationType == typeof(IsolationProvider)))
            return;

        services.AddSingleton<IsolationProvider>();

        IsolationBuilder builder = new IsolationBuilder(services);
        isolationBuilder.Invoke(builder);

        if (services.Count(service => service.ServiceType == typeof(ITenantContext) || service.ServiceType == typeof(IEnvironmentContext)) <
            1)
            throw new NotSupportedException("Tenant isolation and environment isolation use at least one");

        services.AddHttpContextAccessor();

        services
            .TryAddConfigure<IsolationDbConnectionOptions>(Const.DEFAULT_SECTION)
            .AddTransient(typeof(IMiddleware<>), typeof(IsolationMiddleware<>))
            .TryAddSingleton<IDbConnectionStringProvider, IsolationDbContextProvider>();
        services.TryAddScoped(typeof(IIsolationDbConnectionStringProvider), typeof(DefaultDbIsolationConnectionStringProvider));
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

        string name = Options.DefaultName;
        services.AddOptions();
        var configurationSection = configuration.GetSection(sectionName);
        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, configurationSection));
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(
            name,
            configurationSection, _ =>
            {
            }));
        return services;
    }

    private class IsolationProvider
    {
    }
}
