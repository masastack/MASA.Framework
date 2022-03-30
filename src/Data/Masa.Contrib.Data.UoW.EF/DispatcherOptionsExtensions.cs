namespace Masa.Contrib.Data.UoW.EF;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext
    {
        if (options.Services == null)
            throw new ArgumentNullException(nameof(options.Services));

        if (options.Services.Any(service => service.ImplementationType == typeof(UoWProvider)))
            return options;

        options.Services.AddSingleton<UoWProvider>();
        options.Services.TryAddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
        options.Services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        options.Services.TryAddScoped<IConnectionStringProvider, DefaultConnectionStringProvider>();
        options.Services.TryAddSingleton<IDbConnectionStringProvider, DbConnectionStringProvider>();

        options.Services.AddScoped<IUnitOfWork>(serviceProvider => new UnitOfWork<TDbContext>(serviceProvider)
        {
            DisableRollbackOnFailure = disableRollbackOnFailure,
            UseTransaction = useTransaction
        });
        if (options.Services.All(service => service.ServiceType != typeof(MasaDbContextOptions<TDbContext>)))
            options.Services.AddMasaDbContext<TDbContext>(optionsBuilder);

        options.Services.AddScoped<ITransaction, Transaction>();

        return options;
    }

    private class UoWProvider
    {

    }
}
