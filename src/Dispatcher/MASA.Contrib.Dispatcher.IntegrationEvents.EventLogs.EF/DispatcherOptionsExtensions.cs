namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public static class DispatcherOptionsExtensions
{
    /// <summary>
    /// IntegrationEventLogContext is a separate database
    /// </summary>
    /// <param name="options"></param>
    /// <param name="optionsBuilder">Separately specify database configuration for IntegrationEventLogContext</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IDispatcherOptions UseEventLog(
        this IDispatcherOptions options,
        Action<DbContextOptionsBuilder> optionsBuilder)
    {
        if (options.Services == null)
            throw new ArgumentNullException(nameof(options.Services));

        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));

        if (options.Services.Any(service => service.ImplementationType == typeof(EventLogProvider))) return options;
        options.Services.AddSingleton<EventLogProvider>();

        options.Services.AddCustomMasaDbContext<IntegrationEventLogContext>(optionsBuilder);
        return options;
    }

    /// <summary>
    /// User database with IntegrationEventLogContext merge
    /// User-defined DbContext need IntegrationEventLogContext inheritance
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IDispatcherOptions UseEventLog<TDbContext>(
        this IDispatcherOptions options) where TDbContext : IntegrationEventLogContext
    {
        if (options.Services == null)
            throw new ArgumentNullException(nameof(options.Services));

        if (typeof(TDbContext) == typeof(IntegrationEventLogContext))
            throw new NotSupportedException(
                $"{typeof(TDbContext).FullName} must be IntegrationEventLogContext derived classes, or using UseEventLog() replace UseEventLog<{typeof(TDbContext).FullName}>()");

        if (options.Services.Any(service => service.ImplementationType == typeof(EventLogProvider))) return options;
        options.Services.AddSingleton<EventLogProvider>();

        options.Services.TryAddScoped<IntegrationEventLogContext>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>());
        return options;
    }

    private class EventLogProvider
    {
    }
}
