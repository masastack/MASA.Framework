namespace Masa.Contrib.Data.UoW.EF;

public static class DispatcherOptionsExtensions
{
    public static IEventBusBuilder UseUoW<TDbContext>(
        this IEventBusBuilder eventBusBuilder,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
    {
        eventBusBuilder.Services.UseUoW<TDbContext>(nameof(eventBusBuilder.Services), optionsBuilder, disableRollbackOnFailure, useTransaction);
        return eventBusBuilder;
    }

    public static IDispatcherOptions UseUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
    {
        options.Services.UseUoW<TDbContext>(nameof(options.Services), optionsBuilder, disableRollbackOnFailure, useTransaction);
        return options;
    }

    private static IServiceCollection UseUoW<TDbContext>(
        this IServiceCollection services,
        string paramName,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
    {
        if (services == null)
            throw new ArgumentNullException(paramName);

        if (services.Any(service => service.ImplementationType == typeof(UoWProvider)))
            return services;

        services.AddSingleton<UoWProvider>();
        services.TryAddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager<TDbContext>>();
        services.TryAddScoped<IConnectionStringProvider, DefaultConnectionStringProvider>();
        services.TryAddSingleton<IDbConnectionStringProvider, DbConnectionStringProvider>();

        services.AddScoped<IUnitOfWork>(serviceProvider => new UnitOfWork<TDbContext>(serviceProvider)
        {
            DisableRollbackOnFailure = disableRollbackOnFailure,
            UseTransaction = useTransaction
        });
        if (services.All(service => service.ServiceType != typeof(MasaDbContextOptions<TDbContext>)))
        {
            services.TryAddConfigure<MasaDbConnectionOptions>(Const.DEFAULT_SECTION);
            services.AddMasaDbContext<TDbContext>(optionsBuilder);
        }

        services.AddScoped<ITransaction, Transaction>();
        return services;
    }

    /// <summary>
    /// Only consider using MasaConfiguration and database configuration using local configuration
    /// When using MasaConfiguration and the database configuration is stored in ConfigurationAPI, you need to specify the mapping relationship in Configuration by yourself
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        IMasaConfiguration? masaConfiguration = services.BuildServiceProvider().GetService<IMasaConfiguration>();
        if (masaConfiguration == null)
            return services;

        string name = Options.DefaultName;
        services.AddOptions();
        var configurationSection = masaConfiguration.GetConfiguration(SectionTypes.Local).GetSection(sectionName);
        if (!configurationSection.Exists())
            return services;

        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, configurationSection));
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name, configurationSection, _ => { }));
        return services;
    }

    private class UoWProvider
    {
    }
}
