namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteDataExpiresProcessor : IProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<DispatcherOptions> _options;
    private readonly ILogger<DeleteDataExpiresProcessor> _logger;

    public DeleteDataExpiresProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        ILogger<DeleteDataExpiresProcessor> logger)
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
            var logService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
            var expireDate = DateTime.Now.AddSeconds(-_options.Value.ExpireDate);


            //todo: Delete expired events
        }
        await Task.Delay(_options.Value.CleaningExpireInterval, stoppingToken);
        //todo: Delete successfully published and expired messages
    }
}
