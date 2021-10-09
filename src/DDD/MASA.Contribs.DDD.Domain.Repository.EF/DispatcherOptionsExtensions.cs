namespace MASA.Contribs.DDD.Domain.Repository.EF;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseRepository<TDbContext>(
       this IDispatcherOptions options)
       where TDbContext : DbContext
        => options.UseRepository<TDbContext>(AppDomain.CurrentDomain.GetAssemblies());

    public static IDispatcherOptions UseRepository<TDbContext>(
        this IDispatcherOptions options,
        params Assembly[] assemblies)
        where TDbContext : DbContext
    {
        if (options.Services == null)
        {
            throw new ArgumentNullException(nameof(options.Services));
        }

        ServiceCollectionRepositoryExtensions.AddRepository<TDbContext>(options.Services, assemblies);
        return options;
    }
}
