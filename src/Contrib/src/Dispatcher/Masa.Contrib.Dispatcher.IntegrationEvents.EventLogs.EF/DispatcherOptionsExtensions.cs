// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public static class DispatcherOptionsExtensions
{
    /// <summary>
    /// User database with IntegrationEventLogContext merge
    /// User-defined DbContext need IntegrationEventLogContext inheritance
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IDispatcherOptions UseEventLog<TDbContext>(
        this IDispatcherOptions options) where TDbContext : MasaDbContext, IMasaDbContext
    {
        if (options.Services == null)
            throw new ArgumentNullException(nameof(options.Services));

        if (options.Services.Any(service => service.ImplementationType == typeof(EventLogProvider))) return options;

        options.Services.AddSingleton<EventLogProvider>();

        options.Services.TryAddScoped<IIntegrationEventLogService, IntegrationEventLogService>();

        //Add local message table model mapping
        options.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IModelCreatingProvider),
            typeof(IntegrationEventLogModelCreatingProvider), ServiceLifetime.Singleton));
        options.Services.TryAddScoped(typeof(IntegrationEventLogContext),
            serviceProvider => new IntegrationEventLogContext(serviceProvider.GetRequiredService<TDbContext>()));
        return options;
    }

    private class EventLogProvider
    {
    }
}
