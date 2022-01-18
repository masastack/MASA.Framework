namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteExpiresProcessor : IProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeleteExpiresProcessor> _logger;

    public DeleteExpiresProcessor(
        IServiceProvider serviceProvider,
        ILogger<DeleteExpiresProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {

        }

        //todo: Delete successfully published and expired messages
        return Task.CompletedTask;
    }
}
