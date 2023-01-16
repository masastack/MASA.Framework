// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class IntegrationEventHostedService : BackgroundService
{
    private readonly ILogger<IntegrationEventHostedService>? _logger;
    private readonly IEnumerable<IProcessingServer> _processingServers;

    public IntegrationEventHostedService(IEnumerable<IProcessingServer> processingServers,
        ILogger<IntegrationEventHostedService>? logger = null)
    {
        _logger = logger;
        _processingServers = processingServers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger?.LogDebug("----- IntegrationEvent background task is starting");

        foreach (var processingServer in _processingServers)
        {
            await processingServer.ExecuteAsync(stoppingToken);
        }
    }
}
