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
        => options.UseEventLog<TDbContext>(eventLogOptions => eventLogOptions.DisableEntityTypeConfiguration = disableEntityTypeConfiguration);

    /// <summary>
    /// User database with IntegrationEventLogContext merge
    /// User-defined DbContext need IntegrationEventLogContext inheritance
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="options"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IIntegrationEventOptions UseEventLog<TDbContext>(
        this IIntegrationEventOptions options,
        Action<EventLogOptions> configure) where TDbContext : DefaultMasaDbContext, IMasaDbContext
    {
        MasaArgumentException.ThrowIfNull(options.Services);

        if (options.Services.Any(service => service.ImplementationType == typeof(EventLogProvider))) return options;

        options.Services.AddSingleton<EventLogProvider>();

        options.Services.Configure<LocalMessageTableOptions>(option => { option.DbContextType = typeof(TDbContext); });

        var eventLogOptions = new EventLogOptions();
        configure.Invoke(eventLogOptions);
        options.Services.TryAddScoped<IIntegrationEventLogService>(serviceProvider => new IntegrationEventLogService(
            serviceProvider.GetRequiredService<IntegrationEventLogContext>(),
            eventLogOptions.IdGenerator ?? serviceProvider.GetService<IIdGenerator<Guid>>(),
            serviceProvider.GetService<ILogger<IntegrationEventLogService>>()));

        //Add local message table model mapping
        if (!eventLogOptions.DisableEntityTypeConfiguration)
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
