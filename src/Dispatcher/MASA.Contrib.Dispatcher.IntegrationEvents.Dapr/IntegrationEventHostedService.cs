namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class IntegrationEventHostedService : BackgroundService
{
    private readonly IIntegrationEventLogServiceHostedService _hostedService;

    public IntegrationEventHostedService(IIntegrationEventLogServiceHostedService hostedService) => _hostedService = hostedService;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => _hostedService.ExecuteAsync(stoppingToken);
}
