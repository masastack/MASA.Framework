// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaDbContext<TDbContextImplementation>(
        this IServiceCollection services,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        => services.AddMasaDbContext<TDbContextImplementation, Guid>(optionsBuilder, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMasaDbContext<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
        => services
            .AddDbContext<TDbContextImplementation>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation, TUserId>(optionsBuilder, optionsLifetime);

    private static IServiceCollection AddCoreServices<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        services.TryAddConfigure<MasaDbConnectionOptions>();

        MasaDbContextOptionsBuilder masaBuilder = new(services, typeof(TDbContextImplementation), typeof(TUserId));
        optionsBuilder?.Invoke(masaBuilder);
        return services.AddCoreServices<TDbContextImplementation, TUserId>((serviceProvider, efDbContextOptionsBuilder) =>
        {
            if (masaBuilder.EnableSoftDelete)
                efDbContextOptionsBuilder.UseSoftDelete();

            masaBuilder.Builder.Invoke(serviceProvider, efDbContextOptionsBuilder.DbContextOptionsBuilder);
        }, optionsLifetime);
    }

    private static IServiceCollection AddCoreServices<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<IServiceProvider, EFDbContextOptionsBuilder>? optionsBuilder,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        services.TryAddSingleton<IConcurrencyStampProvider, DefaultConcurrencyStampProvider>();
        services.TryAddScoped<IConnectionStringProvider, DefaultConnectionStringProvider>();
        services.TryAddSingleton<IDbConnectionStringProvider, DbConnectionStringProvider>();

        services.TryAdd(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions<TDbContextImplementation>),
                serviceProvider => CreateMasaDbContextOptions<TDbContextImplementation>(serviceProvider, optionsBuilder),
                optionsLifetime));

        services.TryAdd(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions),
                serviceProvider => serviceProvider.GetRequiredService<MasaDbContextOptions<TDbContextImplementation>>(),
                optionsLifetime));

        services.TryAdd(new ServiceDescriptor(typeof(ISaveChangesFilter), typeof(SaveChangeFilter<TDbContextImplementation, TUserId>), ServiceLifetime.Scoped));
        return services;
    }

    private static MasaDbContextOptions<TDbContextImplementation> CreateMasaDbContextOptions<TDbContextImplementation>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, EFDbContextOptionsBuilder>? optionsBuilder)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
    {
        var efDbContextOptionsBuilder = new EFDbContextOptionsBuilder<TDbContextImplementation>();
        optionsBuilder?.Invoke(serviceProvider, efDbContextOptionsBuilder);

        return CreateMasaDbContextOptions<TDbContextImplementation>(
            serviceProvider,
            efDbContextOptionsBuilder.DbContextOptionsBuilder.Options,
            efDbContextOptionsBuilder.EnableSoftDelete);
    }

    private static MasaDbContextOptions<TDbContextImplementation> CreateMasaDbContextOptions<TDbContextImplementation>(
        IServiceProvider serviceProvider,
        DbContextOptions options, bool enableSoftDelete)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        => new(serviceProvider, options, enableSoftDelete);

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services)
        where TOptions : class
        => services.TryAddConfigure<TOptions>(ConnectionStrings.DEFAULT_SECTION);

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
        services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        IConfiguration? configuration = serviceProvider.GetService<IMasaConfiguration>()?.GetConfiguration(SectionTypes.Local) ??
            serviceProvider.GetService<IConfiguration>();
        if (configuration == null)
            return services;

        string name = Options.DefaultName;
        var configurationSection = configuration.GetSection(sectionName);
        if (!configurationSection.Exists())
            return services;

        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, configuration));
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name,
            configuration, _ =>
            {
            }));
        return services;
    }
}
