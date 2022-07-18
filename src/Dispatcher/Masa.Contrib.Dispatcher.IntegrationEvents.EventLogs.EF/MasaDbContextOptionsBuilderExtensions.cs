// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public static class MasaDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// If the local message table is used and the database is migrated using IDesignTimeDbContextFactory<TContext>
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static MasaDbContextOptionsBuilder<TDbContext> MigrateEventLog<TDbContext>(this MasaDbContextOptionsBuilder<TDbContext> builder)
        where TDbContext : MasaDbContext
    {
        builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IModelCreatingProvider),
            typeof(IntegrationEventLogModelCreatingProvider), ServiceLifetime.Singleton));
        builder.Services.TryAddScoped(typeof(IntegrationEventLogContext),
            serviceProvider => new IntegrationEventLogContext(serviceProvider.GetRequiredService<TDbContext>()));
        return builder;
    }
}
