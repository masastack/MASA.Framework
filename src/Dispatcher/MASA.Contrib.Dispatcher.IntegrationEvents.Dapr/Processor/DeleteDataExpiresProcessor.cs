﻿namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteDataExpiresProcessor : ProcessorBase, IProcessor
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
    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var logService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
            var expireDate = (_options.Value.GetCurrentTime?.Invoke() ?? DateTime.UtcNow).AddSeconds(-_options.Value.ExpireDate);
            await logService.DeleteExpiresAsync(expireDate, _options.Value.DeleteBatchCount, stoppingToken);
        }
    }

    public override int SleepTime => _options.Value.CleaningExpireInterval;
}
