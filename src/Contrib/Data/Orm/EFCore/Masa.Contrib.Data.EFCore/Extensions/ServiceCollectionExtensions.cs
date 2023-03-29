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
        where TDbContextImplementation : DbContext, IMasaDbContext
        => services
            .AddDbContext<TDbContextImplementation>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation>(optionsBuilder, optionsLifetime);

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        if (services.Any(service => service.ImplementationType == typeof(MasaDbContextProvider<TDbContextImplementation>)))
            return services;

        services.AddSingleton<MasaDbContextProvider<TDbContextImplementation>>();

        services.TryAddConfigure<ConnectionStrings>();

        MasaDbContextBuilder masaBuilder = new(services, typeof(TDbContextImplementation));
        optionsBuilder?.Invoke(masaBuilder);
        return services.AddCoreServices<TDbContextImplementation>((serviceProvider, efDbContextOptionsBuilder) =>
        {
            masaBuilder.Builder?.Invoke(serviceProvider, efDbContextOptionsBuilder.DbContextOptionsBuilder);
        }, masaBuilder.EnableSoftDelete, masaBuilder.EnablePluralizingTableName, optionsLifetime);
    }

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        bool enablePluralizingTableName,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddSingleton<IConcurrencyStampProvider, DefaultConcurrencyStampProvider>();
        services.AddConnectionStringProvider();

        services.TryAdd(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions<TDbContextImplementation>),
                serviceProvider => CreateMasaDbContextOptions<TDbContextImplementation>(serviceProvider, optionsBuilder, enableSoftDelete, enablePluralizingTableName),
                optionsLifetime));

        services.TryAdd(
            new ServiceDescriptor(
                typeof(MasaDbContextOptions),
                serviceProvider => serviceProvider.GetRequiredService<MasaDbContextOptions<TDbContextImplementation>>(),
                optionsLifetime));

        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>), serviceProvider =>
        {
            var userIdType = serviceProvider.GetService<IOptions<AuditEntityOptions>>()?.Value.UserIdType ?? typeof(Guid);
            var saveChangeFilterType = typeof(SaveChangeFilter<,>).MakeGenericType(typeof(TDbContextImplementation), userIdType);
            return Activator.CreateInstance(saveChangeFilterType, serviceProvider.GetService<IUserContext>())!;
        }, optionsLifetime));
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>), serviceProvider =>
        {
            var userIdType = serviceProvider.GetService<IOptions<AuditEntityOptions>>()?.Value.UserIdType ?? typeof(Guid);
            var softDeleteSaveChangesFilterType =
                typeof(SoftDeleteSaveChangesFilter<,>).MakeGenericType(typeof(TDbContextImplementation), userIdType);
            return Activator.CreateInstance(
                softDeleteSaveChangesFilterType,
                serviceProvider.GetRequiredService<MasaDbContextOptions<TDbContextImplementation>>(),
                serviceProvider.GetRequiredService<TDbContextImplementation>(),
                serviceProvider.GetService<IUserContext>())!;
        }, optionsLifetime));
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>), serviceProvider =>
        {
            var isolationOptions = serviceProvider.GetService<IOptions<IsolationOptions>>();
            if (isolationOptions == null || !isolationOptions.Value.Enable)
            {
                return new EmptySaveFilter<TDbContextImplementation>();
            }

            var genericType = typeof(IsolationSaveChangesFilter<,>).MakeGenericType(typeof(TDbContextImplementation),
                isolationOptions.Value.MultiTenantIdType);
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
            var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
            if (isolationOptions.Value.Enable)
                return serviceProvider.GetRequiredService<IIsolationConnectionStringProviderWrapper>();

            return serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>();
        });
    }

    private static MasaDbContextOptions<TDbContextImplementation> CreateMasaDbContextOptions<TDbContextImplementation>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        bool enablePluralizingTableName)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        var masaDbContextOptionsBuilder = new MasaDbContextOptionsBuilder<TDbContextImplementation>(serviceProvider, enableSoftDelete, enablePluralizingTableName);
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
