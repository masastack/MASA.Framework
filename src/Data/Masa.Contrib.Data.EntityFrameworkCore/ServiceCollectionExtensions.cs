using Masa.BuildingBlocks.Data;

namespace Masa.Contrib.Data.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaDbContext<TDbContext>(
        this IServiceCollection services,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : MasaDbContext
        => services.AddMasaDbContext<TDbContext>(
            (_, masaDbContextOptionsBuilder) => optionsBuilder?.Invoke(masaDbContextOptionsBuilder),
            contextLifetime,
            optionsLifetime);

    public static IServiceCollection AddMasaDbContext<TDbContext>(
        this IServiceCollection services,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : MasaDbContext
        => services
            .AddDbContext<TDbContext>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContext>(optionsBuilder, optionsLifetime);

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : MasaDbContext
    {
        services.TryAddConfigure<MasaDbConnectionOptions>();
        services.TryAddScoped<IConnectionStringProvider, DefaultConnectionStringProvider>();
        services.TryAddScoped(typeof(DataFilter<>));
        services.TryAddScoped<IDataFilter, DataFilter>();
        services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter), typeof(SoftDeleteSaveChangesFilter<TDbContextImplementation>),
            ServiceLifetime.Scoped));

        services.TryAdd(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions<TDbContextImplementation>),
                serviceProvider => CreateMasaDbContextOptions<TDbContextImplementation>(serviceProvider, optionsBuilder),
                optionsLifetime));

        services.Add(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions),
                serviceProvider => serviceProvider.GetRequiredService<MasaDbContextOptions<TDbContextImplementation>>(),
                optionsLifetime));
        return services;
    }

    private static MasaDbContextOptions<TDbContext> CreateMasaDbContextOptions<TDbContext>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder)
        where TDbContext : MasaDbContext
    {
        var masaDbContextOptionsBuilder = new MasaDbContextOptionsBuilder<TDbContext>(serviceProvider);
        optionsBuilder?.Invoke(serviceProvider, masaDbContextOptionsBuilder);

        return CreateMasaDbContextOptions<TDbContext>(
            serviceProvider,
            masaDbContextOptionsBuilder.DbContextOptionsBuilder.Options,
            masaDbContextOptionsBuilder.EnableSoftDelete);
    }

    private static MasaDbContextOptions<TDbContext> CreateMasaDbContextOptions<TDbContext>(IServiceProvider serviceProvider,
        DbContextOptions options, bool enableSoftDelete) where TDbContext : MasaDbContext => new(serviceProvider, options, enableSoftDelete);

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services)
        where TOptions : class
        => services.TryAddConfigure<TOptions>(Const.DEFAULT_SECTION);

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
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name, configurationSection, _ => { }));
        return services;
    }
}
