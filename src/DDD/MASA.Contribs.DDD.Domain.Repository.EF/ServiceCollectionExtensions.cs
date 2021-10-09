namespace MASA.Contribs.DDD.Domain.Repository.EF;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository<TDbContext>(
        this IServiceCollection services,
        params Assembly[] assemblies)
        where TDbContext : DbContext
    {
        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
        {
            throw new Exception("Please add UoW first.");
        }

        return ServiceCollectionRepositoryExtensions.AddRepository<TDbContext>(services, assemblies);
    }
}
