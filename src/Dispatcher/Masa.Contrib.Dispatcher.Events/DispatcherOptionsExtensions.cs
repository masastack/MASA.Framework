namespace Masa.Contrib.Dispatcher.Events;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseEventBus(this IDispatcherOptions options)
        => options.UseEventBus(AppDomain.CurrentDomain.GetAssemblies());

    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options,
        params Assembly[] assemblies)
        => options.UseEventBus(ServiceLifetime.Scoped, assemblies);

    public static IDispatcherOptions UseEventBus(
        this IDispatcherOptions options,
        ServiceLifetime lifetime,
        params Assembly[] assemblies)
    {
        if (options.Services == null)
        {
            throw new ArgumentNullException(nameof(options.Services));
        }

        options.Services.AddEventBus(lifetime, options => options.Assemblies = assemblies);
        return options;
    }
}
