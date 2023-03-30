// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public static class IntegrationEventOptionsExtensions
{
    /// <summary>
    /// User database with IntegrationEventLogContext merge
    /// User-defined DbContext need IntegrationEventLogContext inheritance
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="options"></param>
    /// <param name="disableEntityTypeConfiguration">Disable configuration local message table configuration mapping relationship, default: false（If disabled, you need to configure the mapping manually）</param>
    /// <returns></returns>
    public static IIntegrationEventOptions UseEventLog<TDbContext>(
        this IIntegrationEventOptions options,
        bool disableEntityTypeConfiguration = false) where TDbContext : DefaultMasaDbContext, IMasaDbContext
    {
        MasaArgumentException.ThrowIfNull(options.Services);

        if (options.Services.Any(service => service.ImplementationType == typeof(EventLogProvider))) return options;

        options.Services.AddSingleton<EventLogProvider>();

        options.Services.Configure<LocalMessageTableOptions>(option =>
        {
            option.DbContextType = typeof(TDbContext);
        });

        var integrationEventTypes = options.Assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsClass &&typeof(IIntegrationEvent).IsAssignableFrom(type)).Distinct();
        options.Services.TryAddScoped<IIntegrationEventLogService>(serviceProvider => new IntegrationEventLogService(
            integrationEventTypes,
            serviceProvider.GetRequiredService<IntegrationEventLogContext>(),
            serviceProvider.GetService<ILogger<IntegrationEventLogService>>()));

        //Add local message table model mapping
        if (!disableEntityTypeConfiguration)
            options.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IModelCreatingProvider),
                typeof(IntegrationEventLogModelCreatingProvider),
                ServiceLifetime.Singleton));

        options.Services.TryAddScoped(typeof(IntegrationEventLogContext),
            serviceProvider => new IntegrationEventLogContext(serviceProvider.GetRequiredService<TDbContext>()));
        return options;
    }

    private sealed class EventLogProvider
    {
    }
}
