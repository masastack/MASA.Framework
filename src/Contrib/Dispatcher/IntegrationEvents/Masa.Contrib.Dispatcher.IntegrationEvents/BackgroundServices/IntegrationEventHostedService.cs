// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class IntegrationEventHostedService : BackgroundService
{
    private readonly ILogger<IntegrationEventHostedService>? _logger;
    private readonly IProcessingServer _processingServer;

    public IntegrationEventHostedService(IProcessingServer processingServer, ILogger<IntegrationEventHostedService>? logger = null)
    {
        _logger = logger;
        _processingServer = processingServer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger?.LogDebug("----- IntegrationEvent background task is starting");

        return _processingServer.ExecuteAsync(stoppingToken);
    }
}
