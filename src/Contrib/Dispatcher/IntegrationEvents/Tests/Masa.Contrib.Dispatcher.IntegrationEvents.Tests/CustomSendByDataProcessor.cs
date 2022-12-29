// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

public class CustomSendByDataProcessor : SendByDataProcessor
{
    public CustomSendByDataProcessor(IServiceProvider serviceProvider,
        IOptions<IntegrationEventOptions> options,
        IOptionsMonitor<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        ILogger<SendByDataProcessor>? logger = null)
        : base(serviceProvider, options, masaAppConfigureOptions, logger)
    {
    }

    public Task TestExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        => base.ExecuteAsync(serviceProvider, stoppingToken);
}
