// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.Internal;

internal static class ServiceCollectionRepositoryExtensions
{
    public static IServiceCollection TryAddRepository<TDbContext>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        IEnumerable<Type>? types)
        where TDbContext : DbContext, IMasaDbContext
    {
        MasaArgumentException.ThrowIfNullOrEmptyCollection(assemblies);

        var allTypes = assemblies.SelectMany(assembly => assembly.GetTypes()).ToList();
        var entityTypes = types ?? allTypes.Where(type => type.IsEntity());
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
        var customRepositoryInterfaceTypes = allTypes.Where(type
            => type.GetInterfaces().Any(t => t == repositoryInterfaceType) && type.IsInterface && !type.IsGenericType);
        foreach (var customRepositoryInterfaceType in customRepositoryInterfaceTypes)
        {
            var customRepositoryImplementationTypes =
                allTypes.Where(type => type.IsClass && customRepositoryInterfaceType.IsAssignableFrom(type)).ToList();
            if (customRepositoryImplementationTypes.Count != 1)
            {
                throw new NotSupportedException(
                    $"The number of types of {customRepositoryInterfaceType.FullName} implementation classes must be 1");
            }
            services.TryAddScoped(customRepositoryInterfaceType, customRepositoryImplementationTypes.FirstOrDefault()!);
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
