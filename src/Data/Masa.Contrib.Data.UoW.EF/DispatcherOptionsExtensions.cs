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
            services.AddMasaDbContext<TDbContext>(optionsBuilder);

        services.AddScoped<ITransaction, Transaction>();
        return services;
    }

    private class UoWProvider
    {
    }
}
