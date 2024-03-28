// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Ddd.Domain.Repositories;

public static class DispatcherOptionsExtensions
{
    public static IDomainEventOptions UseRepository<TDbContext>(
        this IDomainEventOptions options,
        params Type[] entityTypes)
        where TDbContext : DbContext, IMasaDbContext
        => options.UseRepository<TDbContext>(entityTypes.Length == 0 ? null : (IEnumerable<Type>)entityTypes);

    public static IDomainEventOptions UseRepository<TDbContext>(
        this IDomainEventOptions options,
        IEnumerable<Type>? entityTypes)
        where TDbContext : DbContext, IMasaDbContext
    {

        MasaArgumentException.ThrowIfNull(options.Services);
#if (NET8_0_OR_GREATER)
        if (options.Services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(RepositoryProvider)))
            return options;
#else
        if (options.Services.Any(service => service.ImplementationType == typeof(RepositoryProvider)))
            return options;
#endif

        options.Services.AddSingleton<RepositoryProvider>();

        if (options.Services.All(service => service.ServiceType != typeof(IUnitOfWork)))
            throw new Exception("Please add UoW first.");

        options.Services.TryAddRepository<TDbContext>(options.Assemblies.Distinct(), entityTypes);
        MasaApp.TrySetServiceCollection(options.Services);
        return options;
    }

    private sealed class RepositoryProvider
    {

    }
}
