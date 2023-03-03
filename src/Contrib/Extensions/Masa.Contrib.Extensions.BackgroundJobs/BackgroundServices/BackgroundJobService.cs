// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.BackgroundServices;

[ExcludeFromCodeCoverage]
public class BackgroundJobService : IHostedService
{
    private readonly IProcessingServer _processingServer;

    public BackgroundJobService(IProcessingServer processingServer)
    {
        _processingServer = processingServer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => _processingServer.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => _processingServer.StopAsync(cancellationToken);
}
