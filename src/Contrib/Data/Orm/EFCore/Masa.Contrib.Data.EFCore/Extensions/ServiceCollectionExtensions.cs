// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaDbContext<TDbContextImplementation>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        => services.AddMasaDbContext<TDbContextImplementation, Guid>(optionsBuilder, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMasaDbContext<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
        => services
            .AddDbContext<TDbContextImplementation>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation, TUserId>(optionsBuilder, optionsLifetime);

    private static IServiceCollection AddCoreServices<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        services.TryAddConfigure<MasaDbConnectionOptions>();

        MasaDbContextBuilder masaBuilder = new(services, typeof(TDbContextImplementation), typeof(TUserId));
        optionsBuilder?.Invoke(masaBuilder);
        return services.AddCoreServices<TDbContextImplementation, TUserId>((serviceProvider, efDbContextOptionsBuilder) =>
        {
            masaBuilder.Builder?.Invoke(serviceProvider, efDbContextOptionsBuilder.DbContextOptionsBuilder);
        }, masaBuilder.EnableSoftDelete, optionsLifetime);
    }

    private static IServiceCollection AddCoreServices<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddSingleton<IConcurrencyStampProvider, DefaultConcurrencyStampProvider>();
        services.TryAddScoped<IConnectionStringProvider, DefaultConnectionStringProvider>();
        services.TryAddScoped<IDbConnectionStringProvider, DbConnectionStringProvider>();

        services.TryAdd(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions<TDbContextImplementation>),
                serviceProvider => CreateMasaDbContextOptions<TDbContextImplementation>(serviceProvider, optionsBuilder, enableSoftDelete),
                optionsLifetime));

        services.TryAdd(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions),
                serviceProvider => serviceProvider.GetRequiredService<MasaDbContextOptions<TDbContextImplementation>>(),
                optionsLifetime));

        services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            typeof(SaveChangeFilter<TDbContextImplementation, TUserId>), optionsLifetime));
        services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            typeof(SoftDeleteSaveChangesFilter<TDbContextImplementation, TUserId>), optionsLifetime));
        return services;
    }

    private static MasaDbContextOptions<TDbContextImplementation> CreateMasaDbContextOptions<TDbContextImplementation>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete)
        where TDbContextImplementation : MasaDbContext, IMasaDbContext
    {
        var masaDbContextOptionsBuilder = new MasaDbContextOptionsBuilder<TDbContextImplementation>(serviceProvider, enableSoftDelete);
        optionsBuilder?.Invoke(serviceProvider, masaDbContextOptionsBuilder);

        return masaDbContextOptionsBuilder.MasaOptions;
    }

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services)
        where TOptions : class
        => services.AddConfigure<TOptions>(ConnectionStrings.DEFAULT_SECTION, isRoot: true);
}
