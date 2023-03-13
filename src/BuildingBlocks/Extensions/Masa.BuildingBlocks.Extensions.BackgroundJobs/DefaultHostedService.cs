// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

[ExcludeFromCodeCoverage]
public class DefaultHostedService : IProcessingServer
{
    private readonly IEnumerable<IProcessor> _processors;
    private readonly ILogger<DefaultHostedService>? _logger;

    public DefaultHostedService(IEnumerable<IProcessor> processors, ILogger<DefaultHostedService>? logger = null)
    {
        _processors = processors;
        _logger = logger;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("-----The background task is start, the number of tasks: {Number}", _processors.Count());

        foreach (var processor in _processors)
        {
            _logger?.LogDebug("-----The background task is start, processor: {Processor}", processor.GetType().FullName);
            await processor.StartAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("-----The background task is stop, the number of tasks: {Number}", _processors.Count());

        foreach (var processor in _processors)
        {
            _logger?.LogDebug("-----The background task is stop, processor: {Processor}", processor.GetType().FullName);

            await processor.StopAsync(cancellationToken);
        }
    }
}
