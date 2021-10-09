namespace MASA.Contrib.Data.Uow.EF;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUoW<TDbContext>(
        this IServiceCollection services,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null)
        where TDbContext : MasaDbContext
    {
        services.AddLogging();
        services.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();
        if (services.All(service => service.ServiceType != typeof(MasaDbContextOptions<TDbContext>)))
        {
            services.AddMasaDbContext<TDbContext>(optionsBuilder);
        }

        services.AddScoped<ITransaction, Transaction>();
        return services;
    }
}
