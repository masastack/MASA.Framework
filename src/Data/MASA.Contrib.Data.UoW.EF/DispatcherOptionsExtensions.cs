namespace MASA.Contrib.Data.UoW.EF;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null)
        where TDbContext : MasaDbContext
    {
        if (options.Services == null)
        {
            throw new ArgumentNullException(nameof(options.Services));
        }

        if (options.Services.Any(service => service.ImplementationType == typeof(UoWProvider))) return options;
        options.Services.AddSingleton<UoWProvider>();

        options.Services.AddLogging();
        options.Services.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();
        if (options.Services.All(service => service.ServiceType != typeof(MasaDbContextOptions<TDbContext>)))
        {
            options.Services.AddMasaDbContext<TDbContext>(optionsBuilder);
        }

        options.Services.AddScoped<ITransaction, Transaction>();

        return options;
    }

    private class UoWProvider
    {

    }
}

