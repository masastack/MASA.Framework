namespace Masa.Contrib.Dispatcher.Events;

public class EventBusBuilder
{
    public IServiceCollection Services { get; }

    public EventBusBuilder(IServiceCollection services) => Services = services;
}
