namespace Masa.Contrib.Dispatcher.Events;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options)
        => options.UseEventBus(ServiceLifetime.Scoped);

    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options,
        ServiceLifetime lifetime)
    {
        ArgumentNullException.ThrowIfNull(options.Services,nameof(options.Services));

        options.Services.AddEventBus(options.Assemblies, lifetime);
        return options;
    }
}
