namespace Masa.Contrib.Ddd.Domain.Repository.EF;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseRepository<TDbContext>(
        this IDispatcherOptions options)
        where TDbContext : DbContext
    {
        if (options.Services == null)
            throw new ArgumentNullException(nameof(options.Services));

        if (options.Services.Any(service => service.ImplementationType == typeof(RepositoryProvider)))
            return options;

        options.Services.AddSingleton<RepositoryProvider>();

        if (options.Services.All(service => service.ServiceType != typeof(IUnitOfWork)))
            throw new Exception("Please add UoW first.");

        options.Services.TryAddRepository<TDbContext>(options.Assemblies);
        return options;
    }

    private class RepositoryProvider
    {

    }
}
