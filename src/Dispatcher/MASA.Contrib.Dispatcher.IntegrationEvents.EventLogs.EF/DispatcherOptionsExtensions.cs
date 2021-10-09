using MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Internal;

namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public static class DispatcherOptionsExtensions
{
    /// <summary>
    /// IntegrationEventLogContext a single database
    /// </summary>
    /// <param name="options"></param>
    /// <param name="optionsBuilder">If the service has only one DbContext, configure it when the Uow is used. Otherwise, configure it separately for the log service</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IDispatcherOptions UseEventLog(
        this IDispatcherOptions options,
        Action<DbContextOptionsBuilder>? optionsBuilder = null)
    {
        if (options.Services == null)
        {
            throw new ArgumentNullException(nameof(options.Services));
        }
        DbContextExtensions.AddCustomMasaDbContext<IntegrationEventLogContext>(options.Services, optionsBuilder);
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
        if (typeof(TDbContext) == typeof(IntegrationEventLogContext))
        {
            throw new NotSupportedException($"{typeof(TDbContext).FullName} must be IntegrationEventLogContext derived classes, or using UseEventLog() replace UseEventLog<{typeof(TDbContext).FullName}>()");
        }
        options.Services.TryAddScoped<IntegrationEventLogContext>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>());
        return options;
    }
}
