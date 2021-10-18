namespace MASA.Contrib.DDD.Domain.Repository.EF.Internal;

internal static class ServiceCollectionRepositoryExtensions
{
    public static IServiceCollection TryAddRepository<TDbContext>(
       this IServiceCollection services,
       params Assembly[] assemblies)
       where TDbContext : DbContext
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        var allTypes = assemblies.SelectMany(assembly => assembly.GetTypes());
        var entityTypes = allTypes.Where(type => type.IsAggregateRootEntity());
        foreach (var entityType in entityTypes)
        {
            var repositoryInterfaceType = typeof(IRepository<>).MakeGenericType(entityType);

            services.TryAddAddDefaultRepository(repositoryInterfaceType, GetRepositoryImplementationType(typeof(TDbContext), entityType));

            services.TryAddCustomRepository(repositoryInterfaceType, allTypes.ToArray());
        }
        return services;
    }

    private static bool IsAggregateRootEntity(this Type type)
        => type.IsClass && !type.IsGenericType && !type.IsAbstract && type != typeof(AggregateRoot) && type != typeof(Entity) && typeof(IAggregateRoot).IsAssignableFrom(type);

    private static IServiceCollection TryAddCustomRepository(this IServiceCollection services, Type repositoryInterfaceType, Type[] allTypes)
    {
        var customerRepositoryInterfaceTypes = allTypes.Where(type => type.GetInterfaces().Any(t => t == repositoryInterfaceType) && type.IsInterface && !type.IsGenericType);
        foreach (var customerRepositoryInterfaceType in customerRepositoryInterfaceTypes)
        {
            var customerRepositoryImplementationTypes = allTypes.Where(type => type.IsClass && customerRepositoryInterfaceType.IsAssignableFrom(type)).ToList();
            if (customerRepositoryImplementationTypes.Count != 1)
            {
                throw new NotSupportedException($"The number of types of {customerRepositoryInterfaceType.FullName} implementation classes must be 1");
            }
            services.TryAddScoped(customerRepositoryInterfaceType, customerRepositoryImplementationTypes.FirstOrDefault()!);
        }
        return services;
    }

    private static IServiceCollection TryAddAddDefaultRepository(this IServiceCollection services, Type repositoryInterfaceType, Type repositoryImplementationType)
    {
        if (repositoryInterfaceType.IsAssignableFrom(repositoryImplementationType))
        {
            services.TryAddScoped(repositoryInterfaceType, repositoryImplementationType);
        }
        return services;
    }

    public static Type GetRepositoryImplementationType(Type dbContextType, Type entityType)
        => typeof(Repository<,>).MakeGenericType(dbContextType, entityType);
}
