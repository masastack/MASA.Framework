namespace Masa.Contrib.Isolation.UoW.EF;

public static class DispatcherOptionsExtensions
{
    public static IEventBusBuilder UseIsolationUoW<TDbContext>(
        this IEventBusBuilder eventBusBuilder,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder,
        Action<IsolationBuilder> isolationBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
    {
        eventBusBuilder.Services.UseIsolationUoW(nameof(eventBusBuilder.Services), isolationBuilder);
        return eventBusBuilder.UseUoW<TDbContext>(optionsBuilder, disableRollbackOnFailure, useTransaction);
    }

    public static IDispatcherOptions UseIsolationUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder,
        Action<IsolationBuilder> isolationBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
    {
        options.Services.UseIsolationUoW(nameof(options.Services), isolationBuilder);
        return options.UseUoW<TDbContext>(optionsBuilder, disableRollbackOnFailure, useTransaction);
    }

    private static IServiceCollection UseIsolationUoW(
        this IServiceCollection services,
        string paramName,
        Action<IsolationBuilder> isolationBuilder)
    {
        ArgumentNullException.ThrowIfNull(services, paramName);
        ArgumentNullException.ThrowIfNull(isolationBuilder);

        if (services.Any(service => service.ImplementationType == typeof(IsolationUoWProvider)))
            return services;

        services.AddSingleton<IsolationUoWProvider>();

        IsolationBuilder builder = new IsolationBuilder(services);
        isolationBuilder.Invoke(builder);

        if (services.Count(service => service.ServiceType == typeof(ITenantContext) || service.ServiceType == typeof(IEnvironmentContext)) < 1)
            throw new NotSupportedException("Tenant isolation and environment isolation use at least one");

        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = builder.TenantKey;
            option.EnvironmentKey = builder.EnvironmentKey;
        });

        services.AddHttpContextAccessor();

        if (services.Any(service => service.ServiceType == typeof(ITenantContext)))
            services.AddScoped<IIsolationMiddleware>(serviceProvider => new MultiTenantMiddleware(serviceProvider, builder.TenantParsers));

        if (services.Any(service => service.ServiceType == typeof(IEnvironmentContext)))
            services.AddScoped<IIsolationMiddleware>(serviceProvider => new MultiEnvironmentMiddleware(serviceProvider, builder.EnvironmentParsers));

        services.AddTransient(typeof(IMiddleware<>), typeof(IsolationMiddleware<>));
        services.TryAddSingleton<IDbConnectionStringProvider, IsolationDbContextProvider>();
        services.TryAddConfigure<IsolationDbConnectionOptions>(Const.DEFAULT_SECTION);
        services.TryAddScoped(typeof(IConnectionStringProvider), typeof(DefaultConnectionStringProvider));
        return services;
    }

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        IConfiguration? configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        if (configuration == null)
            return services;

        string name = Options.DefaultName;
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

    private class IsolationUoWProvider
    {
    }
}
