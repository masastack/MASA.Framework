// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static void UseIsolationUoW<TDbContext, TTenantId>(this IServiceCollection services)
        where TTenantId : IComparable
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
        => services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContext>),
            typeof(IsolationSaveChangesFilter<TDbContext, TTenantId>),
            ServiceLifetime.Scoped));
}
