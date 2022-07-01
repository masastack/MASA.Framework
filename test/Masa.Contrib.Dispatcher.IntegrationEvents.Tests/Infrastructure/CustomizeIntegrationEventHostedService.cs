// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Infrastructure;

public class CustomizeIntegrationEventHostedService : IntegrationEventHostedService
{
    public CustomizeIntegrationEventHostedService(
        IProcessingServer processingServer, ILoggerFactory? loggerFactory = null) :
        base(processingServer, loggerFactory?.CreateLogger<IntegrationEventHostedService>())
    {
    }

    public Task TestExecuteAsync(CancellationToken stoppingToken)
    {
        return ExecuteAsync(stoppingToken);
    }
}
