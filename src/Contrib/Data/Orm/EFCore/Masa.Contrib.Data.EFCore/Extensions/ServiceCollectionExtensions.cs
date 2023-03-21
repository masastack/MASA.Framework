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
        where TDbContextImplementation : MasaDbContext<TDbContextImplementation>, IMasaDbContext
        => services.AddMasaDbContext<TDbContextImplementation, Guid>(optionsBuilder, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMasaDbContext<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : MasaDbContext<TDbContextImplementation>, IMasaDbContext
        where TUserId : IComparable
        => services
            .AddDbContext<TDbContextImplementation>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation, TUserId>(optionsBuilder, optionsLifetime);

    private static IServiceCollection AddCoreServices<TDbContextImplementation, TUserId>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : MasaDbContext<TDbContextImplementation>, IMasaDbContext
        where TUserId : IComparable
    {
        if (services.Any(service => service.ImplementationType == typeof(MasaDbContextProvider<TDbContextImplementation>)))
            return services;

        services.AddSingleton<MasaDbContextProvider<TDbContextImplementation>>();

        services.TryAddConfigure<ConnectionStrings>();

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
        where TDbContextImplementation : MasaDbContext<TDbContextImplementation>, IMasaDbContext
        where TUserId : IComparable
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddSingleton<IConcurrencyStampProvider, DefaultConcurrencyStampProvider>();
        services.AddConnectionStringProvider();

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

        services.TryAddSingleton<MultiTenantProvider>();
        services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            typeof(SaveChangeFilter<TDbContextImplementation, TUserId>), optionsLifetime));
        services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            typeof(SoftDeleteSaveChangesFilter<TDbContextImplementation, TUserId>), optionsLifetime));
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>), serviceProvider =>
        {
            var isolationOptions = serviceProvider.GetService<IOptions<IsolationOptions>>();
            if (isolationOptions == null || !isolationOptions.Value.Enable)
            {
                return new EmptySaveFilter<TDbContextImplementation>();
            }
            var genericType = typeof(IsolationSaveChangesFilter<,>).MakeGenericType(typeof(TDbContextImplementation), serviceProvider.GetService<MultiTenantProvider>()?.MultiTenantIdType!);
            var isolationSaveChangesFilter = Activator.CreateInstance(genericType,
                new object?[]
                {
                    serviceProvider
                });
            return (isolationSaveChangesFilter as ISaveChangesFilter<TDbContextImplementation>)!;
        }, optionsLifetime));
        return services;
    }

    private static void AddConnectionStringProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IConnectionStringProviderWrapper, DefaultConnectionStringProvider>();
        services.TryAddScoped<IIsolationConnectionStringProviderWrapper, DefaultIsolationConnectionStringProvider>();
        services.TryAddScoped<IConnectionStringProvider>(serviceProvider =>
        {
            var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
            if (isolationOptions.Value.Enable)
                serviceProvider.GetRequiredService<IIsolationConnectionStringProviderWrapper>();

            return serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>();
        });
    }

    private static MasaDbContextOptions<TDbContextImplementation> CreateMasaDbContextOptions<TDbContextImplementation>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete)
        where TDbContextImplementation : MasaDbContext<TDbContextImplementation>, IMasaDbContext
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
