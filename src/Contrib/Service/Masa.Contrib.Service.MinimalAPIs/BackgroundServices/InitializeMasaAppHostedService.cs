// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.BackgroundServices;

public class InitializeMasaAppHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public InitializeMasaAppHostedService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MasaApp.TrySetRootServices(_serviceProvider, true);
        return Task.CompletedTask;
    }
}
