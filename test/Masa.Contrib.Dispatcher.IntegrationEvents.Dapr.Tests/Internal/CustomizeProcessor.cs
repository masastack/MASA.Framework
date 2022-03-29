namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Internal;

public class CustomizeProcessor : ProcessorBase
{
    internal static int Times = 0;

    public override int Delay => 2;

    public CustomizeProcessor(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Times++;
        return Task.CompletedTask;
    }
}
