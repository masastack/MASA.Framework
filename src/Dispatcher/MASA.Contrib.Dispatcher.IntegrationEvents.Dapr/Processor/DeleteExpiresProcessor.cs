namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteExpiresProcessor : IProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<DispatcherOptions> _options;
    private readonly ILogger<DeleteExpiresProcessor> _logger;

    public DeleteExpiresProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        ILogger<DeleteExpiresProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {

        }
        await Task.Delay(_options.Value.CleaningExpireInterval, stoppingToken);
        //todo: Delete successfully published and expired messages
    }
}
