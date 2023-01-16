// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Infrastructure;

public class CustomIntegrationEventHostedService : IntegrationEventHostedService
{
    public CustomIntegrationEventHostedService(
        IEnumerable<IProcessingServer> processingServers, ILoggerFactory? loggerFactory = null) :
        base(processingServers, loggerFactory?.CreateLogger<IntegrationEventHostedService>())
    {
    }

    public Task TestExecuteAsync(CancellationToken stoppingToken)
    {
        return ExecuteAsync(stoppingToken);
    }
}
