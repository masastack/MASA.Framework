namespace Masa.Contrib.Ddd.Domain.Repository.EF.Internal;

internal static class ServiceCollectionRepositoryExtensions
{
    public static IServiceCollection TryAddRepository<TDbContext>(
        this IServiceCollection services,
        params Assembly[] assemblies)
        where TDbContext : DbContext
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        var allTypes = assemblies.SelectMany(assembly => assembly.GetTypes()).ToList();
        var entityTypes = allTypes.Where(type => type.IsEntity());
        foreach (var entityType in entityTypes)
        {
            var repositoryInterfaceType = typeof(IRepository<>).MakeGenericType(entityType);
            services.TryAddAddDefaultRepository(repositoryInterfaceType, GetRepositoryImplementationType(typeof(TDbContext), entityType));
            services.TryAddCustomRepository(repositoryInterfaceType, allTypes);

            if (typeof(IEntity<>).IsGenericInterfaceAssignableFrom(entityType))
            {
                var fieldType = entityType.GetProperty("Id")!.PropertyType;
                repositoryInterfaceType = typeof(IRepository<,>).MakeGenericType(entityType, fieldType);
                services.TryAddAddDefaultRepository(repositoryInterfaceType,
                    GetRepositoryImplementationType(typeof(TDbContext), entityType, fieldType));
                services.TryAddCustomRepository(repositoryInterfaceType, allTypes);
            }
        }

        return services;
    }

    private static bool IsEntity(this Type type)
        => type.IsClass && !type.IsGenericType && !type.IsAbstract && typeof(IEntity).IsAssignableFrom(type);

    private static void TryAddCustomRepository(this IServiceCollection services, Type repositoryInterfaceType, List<Type> allTypes)
    {
        var customerRepositoryInterfaceTypes = allTypes.Where(type
            => type.GetInterfaces().Any(t => t == repositoryInterfaceType) && type.IsInterface && !type.IsGenericType);
        foreach (var customerRepositoryInterfaceType in customerRepositoryInterfaceTypes)
        {
            var customerRepositoryImplementationTypes =
                allTypes.Where(type => type.IsClass && customerRepositoryInterfaceType.IsAssignableFrom(type)).ToList();
            if (customerRepositoryImplementationTypes.Count != 1)
            {
                throw new NotSupportedException(
                    $"The number of types of {customerRepositoryInterfaceType.FullName} implementation classes must be 1");
            }
            services.TryAddScoped(customerRepositoryInterfaceType, customerRepositoryImplementationTypes.FirstOrDefault()!);
        }
    }

    private static void TryAddAddDefaultRepository(this IServiceCollection services, Type repositoryInterfaceType,
        Type repositoryImplementationType)
    {
        if (repositoryInterfaceType.IsAssignableFrom(repositoryImplementationType))
        {
            services.TryAddScoped(repositoryInterfaceType, repositoryImplementationType);
        }
    }

    private static Type GetRepositoryImplementationType(Type dbContextType, Type entityType)
        => typeof(Repository<,>).MakeGenericType(dbContextType, entityType);

    private static Type GetRepositoryImplementationType(Type dbContextType, Type entityType, Type keyType)
        => typeof(Repository<,,>).MakeGenericType(dbContextType, entityType, keyType);
}
