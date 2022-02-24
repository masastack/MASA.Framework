namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Internal;

internal static class DbContextExtensions
{
    internal static IServiceCollection AddCustomMasaDbContext<TDbContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> contextAction)
        where TDbContext : MasaDbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        contextAction.Invoke(optionsBuilder);

        services.AddDbContext<TDbContext>();
        services.TryAddScoped(typeof(MasaDbContextOptions<TDbContext>), serviceProvider =>
        {
            return CreateMasaDbContextOptions<TDbContext>(optionsBuilder.Options);
        });
        return services;
    }

    private static MasaDbContextOptions<TDbContext> CreateMasaDbContextOptions<TDbContext>(
        DbContextOptions originOptions)
        where TDbContext : MasaDbContext
    {
        return new MasaDbContextOptions<TDbContext>(originOptions, new List<QueryFilterProvider>(), new List<SaveChangesFilter>());
    }
}
