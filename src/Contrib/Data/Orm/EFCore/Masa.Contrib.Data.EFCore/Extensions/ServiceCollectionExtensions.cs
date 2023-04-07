// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
        where TDbContextImplementation : DbContext, IMasaDbContext
        => services
            .AddDbContext<TDbContextImplementation>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation>(optionsBuilder, contextLifetime, optionsLifetime);

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<MasaDbContextBuilder>? optionsBuilder,
        ServiceLifetime contextLifetime,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        if (services.Any(service => service.ImplementationType == typeof(MasaDbContextProvider<TDbContextImplementation>)))
            return services;

        services.AddSingleton<MasaDbContextProvider<TDbContextImplementation>>();

        services.Replace(new ServiceDescriptor(typeof(TDbContextImplementation), serviceProvider =>
        {
            //todo: Temporarily use reflection, and then change to expression tree
            var dbContextType = typeof(TDbContextImplementation);
            var constructorInfo = dbContextType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .MaxBy(c => c.GetParameters().Length);
            MasaArgumentException.ThrowIfNull(constructorInfo);

            var parameters = new List<object?>();
            foreach (var parameter in constructorInfo.GetParameters())
            {
                parameters.Add(serviceProvider.GetService(parameter.ParameterType));
            }

            var dbContext = parameters.Count > 0
                ? Activator.CreateInstance(typeof(TDbContextImplementation), parameters.ToArray()) as DefaultMasaDbContext
                : Activator.CreateInstance(typeof(TDbContextImplementation)) as DefaultMasaDbContext;
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
            }
        }, masaBuilder.EnableSoftDelete, optionsLifetime);
    }

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IMasaDbContext
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
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
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
            var isolationSaveChangesFilter = Activator.CreateInstance(genericType, serviceProvider);
            return (isolationSaveChangesFilter as ISaveChangesFilter<TDbContextImplementation>)!;
        }, optionsLifetime));
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
