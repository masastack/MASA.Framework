namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Internal;

public class CustomizeProcessor : ProcessorBase
{
    internal static int Times = 0;
    
    public override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Times++;
        return Task.CompletedTask;
    }

    public override int Delay => 2;
}
