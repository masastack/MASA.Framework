// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation")]

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable S1135
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaDbContext<TDbContextImplementation>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : DefaultMasaDbContext, IMasaDbContext
        => services
            .AddDbContext<TDbContextImplementation>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation>(optionsBuilder, contextLifetime, optionsLifetime);

    /// <summary>
    /// Initialize cache data, only for testing
    /// </summary>
    internal static void InitializeCacheData(this IServiceCollection services)
    {
        DbContextExtensions.InitializeCacheData();
    }

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder,
        ServiceLifetime contextLifetime,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DefaultMasaDbContext, IMasaDbContext
    {

#if (NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(MasaDbContextProvider<TDbContextImplementation>)))
            return services;
#else
        if (services.Any(service => service.ImplementationType == typeof(MasaDbContextProvider<TDbContextImplementation>)))
            return services;
#endif

        services.AddSingleton<MasaDbContextProvider<TDbContextImplementation>>();

        services.Replace(new ServiceDescriptor(typeof(TDbContextImplementation), serviceProvider =>
        {
            var dbContext = DbContextExtensions.CreateDbContext<TDbContextImplementation>(serviceProvider);
            MasaArgumentException.ThrowIfNull(dbContext);

            dbContext.TryInitializeMasaDbContextOptions(serviceProvider.GetService<MasaDbContextOptions<TDbContextImplementation>>());
            return dbContext;
        }, contextLifetime));
        services.TryAddConfigure<ConnectionStrings>();

        MasaDbContextBuilder masaBuilder = new(services, typeof(TDbContextImplementation));
        optionsBuilder?.Invoke(masaBuilder);
        return services.AddCoreServices<TDbContextImplementation>((serviceProvider, efDbContextOptionsBuilder) =>
        {
            if (masaBuilder.Builder != null)
            {
                efDbContextOptionsBuilder.DbContextOptionsBuilder.UseApplicationServiceProvider(serviceProvider);
                efDbContextOptionsBuilder.DbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                masaBuilder.Builder.Invoke(serviceProvider, efDbContextOptionsBuilder.DbContextOptionsBuilder);

                foreach (var dbContextOptionsBuilder in masaBuilder.DbContextOptionsBuilders)
                {
                    dbContextOptionsBuilder.Invoke(efDbContextOptionsBuilder.DbContextOptionsBuilder);
                }
            }
        }, masaBuilder.EnableSoftDelete, optionsLifetime);
    }

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DefaultMasaDbContext, IMasaDbContext
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddSingleton<IConcurrencyStampProvider, DefaultConcurrencyStampProvider>();

        return services
            .AddMasaDbContextOptions<TDbContextImplementation>(optionsBuilder, enableSoftDelete, optionsLifetime)
            .AddConnectionStringProvider()
            .AddFilter<TDbContextImplementation>(optionsLifetime);
    }

    private static IServiceCollection AddConnectionStringProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IConnectionStringProviderWrapper, DefaultConnectionStringProvider>();
        services.TryAddScoped<IIsolationConnectionStringProviderWrapper>(serviceProvider =>
            new DefaultIsolationConnectionStringProvider(
                serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>(),
                serviceProvider.GetRequiredService<IIsolationConfigProvider>(),
                serviceProvider.GetService<IUnitOfWorkAccessor>(),
                serviceProvider.GetService<IMultiEnvironmentContext>(),
                serviceProvider.GetService<IMultiTenantContext>(),
                serviceProvider.GetService<ILogger<DefaultIsolationConnectionStringProvider>>()));
        services.TryAddScoped<IConnectionStringProvider>(serviceProvider =>
        {
            if (serviceProvider.EnableIsolation())
                return serviceProvider.GetRequiredService<IIsolationConnectionStringProviderWrapper>();

            return serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>();
        });
        return services;
    }

    private static IServiceCollection AddFilter<TDbContextImplementation>(
        this IServiceCollection services,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DefaultMasaDbContext, IMasaDbContext
    {
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            DbContextExtensions.CreateSaveChangesFilter<TDbContextImplementation>, optionsLifetime));
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            DbContextExtensions.CreateSoftDeleteSaveChangesFilter<TDbContextImplementation>, optionsLifetime));
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            DbContextExtensions.CreateIsolationSaveChangesFilter<TDbContextImplementation>, optionsLifetime));
        return services;
    }

    private static IServiceCollection AddMasaDbContextOptions<TDbContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
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
        return services;
    }

    private static MasaDbContextOptions<TDbContextImplementation> CreateMasaDbContextOptions<TDbContextImplementation>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        var masaDbContextOptionsBuilder = new MasaDbContextOptionsBuilder<TDbContextImplementation>(serviceProvider, enableSoftDelete);
        optionsBuilder?.Invoke(serviceProvider, masaDbContextOptionsBuilder);

        return masaDbContextOptionsBuilder.MasaOptions;
    }

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services)
        where TOptions : class
        => services.AddConfigure<TOptions>(ConnectionStrings.DEFAULT_SECTION, isRoot: false);

#pragma warning disable S2326
#pragma warning disable S2094
    private sealed class MasaDbContextProvider<TDbContext>
    {
    }
#pragma warning restore S2094
#pragma warning restore S2326
}
#pragma warning restore S1135
