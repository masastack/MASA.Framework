// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class DeletePublishedExpireEventProcessor : ProcessorBase
{
    private readonly IOptions<IntegrationEventOptions> _options;

    public override int Delay => _options.Value.CleaningExpireInterval;

    public DeletePublishedExpireEventProcessor(IServiceProvider serviceProvider, IOptions<IntegrationEventOptions> options)
        : base(serviceProvider)
    {
        _options = options;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var logService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        var expireDate = (_options.Value.GetCurrentTime?.Invoke() ?? DateTime.UtcNow).AddSeconds(-_options.Value.PublishedExpireTime);
        await logService.DeleteExpiresAsync(expireDate, _options.Value.DeleteBatchCount, stoppingToken);
    }
}
