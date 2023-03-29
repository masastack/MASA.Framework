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
            var dbContext = Activator.CreateInstance(typeof(TDbContextImplementation), parameters.ToArray()) as MasaDbContext;
            MasaArgumentException.ThrowIfNull(dbContext);

            dbContext.TrySetMasaDbContextOptions(serviceProvider.GetService<MasaDbContextOptions<TDbContextImplementation>>());
            return dbContext;
        }, contextLifetime));
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
                serviceProvider => CreateMasaDbContextOptions<TDbContextImplementation>(serviceProvider, optionsBuilder, enableSoftDelete,
                    enablePluralizingTableName),
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
        return services;
    }

    private static void AddConnectionStringProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IConnectionStringProviderWrapper, DefaultConnectionStringProvider>();
        services.TryAddScoped<IConnectionStringProvider>(serviceProvider => serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>());
    }

    private static MasaDbContextOptions<TDbContextImplementation> CreateMasaDbContextOptions<TDbContextImplementation>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, MasaDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        bool enablePluralizingTableName)
        where TDbContextImplementation : DbContext, IMasaDbContext
    {
        var masaDbContextOptionsBuilder =
            new MasaDbContextOptionsBuilder<TDbContextImplementation>(serviceProvider, enableSoftDelete, enablePluralizingTableName);
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
