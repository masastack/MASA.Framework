namespace Masa.Contrib.Ddd.Domain.Repository.EF.Internal;

internal static class ServiceCollectionRepositoryExtensions
{
    /// <summary>
    /// The relationship between entity and keys
    /// </summary>
    public static Dictionary<Type, string[]> Relations = new();

    public static IServiceCollection TryAddRepository<TDbContext>(
       this IServiceCollection services,
       params Assembly[] assemblies)
       where TDbContext : DbContext
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        var allTypes = assemblies.SelectMany(assembly => assembly.GetTypes());
        var entityTypes = allTypes.Where(type => type.IsAggregateRootEntity());
        foreach (var entityType in entityTypes)
        {
            var repositoryInterfaceType = typeof(IRepository<>).MakeGenericType(entityType);

            services.TryAddAddDefaultRepository(repositoryInterfaceType, GetRepositoryImplementationType(typeof(TDbContext), entityType));

            services.TryAddCustomRepository(repositoryInterfaceType, allTypes.ToArray());

            var keys = GetKeys(entityType);
            CheckKeys(entityType, keys);
            Relations.TryAdd(entityType, keys);
        }

        return services;
    }

    private static string[] GetKeys(Type entityType)
    {
        IAggregateRoot aggregateRoot;
        try
        {
            var constructorInfo = entityType
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(con => !con.CustomAttributes.Any());

            if (constructorInfo == null)
                throw new ArgumentNullException("The entity needs to have an empty constructor");

            aggregateRoot = (IAggregateRoot)Activator.CreateInstance(entityType, constructorInfo.IsPrivate)!;
        }
        catch (Exception)
        {
            throw new ArgumentNullException("The entity needs to have an empty constructor");
        }

        var keys = aggregateRoot.GetKeys().Select(k => k.Name).ToArray();
        if (keys.Length != keys.Where(key => !string.IsNullOrEmpty(key)).Distinct().Count())
            throw new ArgumentException("The joint primary key cannot be empty");

        return keys;
    }

    /// <summary>
    /// Check if the combined primary key is correct
    /// </summary>
    private static void CheckKeys(Type entityType, string[] fields)
    {
        foreach (var field in fields)
        {
            if (!entityType.GetProperties().Any(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase))!)
                throw new ArgumentException("Check if the combined primary key is correct");
        }
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
