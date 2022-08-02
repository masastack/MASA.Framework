// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public interface IProcessor
{
    Task ExecuteAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Easy to switch between background tasks
    /// </summary>
    /// <param name="delay">unit: seconds</param>
    /// <returns></returns>
    Task DelayAsync(int delay);
}
