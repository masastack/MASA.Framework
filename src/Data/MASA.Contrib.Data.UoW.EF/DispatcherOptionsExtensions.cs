namespace MASA.Contrib.Data.UoW.EF;

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

        options.Services.AddScoped<IUnitOfWork>(serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<TDbContext>();
            var logger = serviceProvider.GetService<ILogger<UnitOfWork<TDbContext>>>();
            return new UnitOfWork<TDbContext>(dbContext, logger)
            {
                DisableRollbackOnFailure = disableRollbackOnFailure,
                UseTransaction = useTransaction
            };
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

