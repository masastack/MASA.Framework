// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Internal;

internal class WorkerIdBackgroundServices : BackgroundService
{
    private readonly IWorkerProvider _workerProvider;

    public WorkerIdBackgroundServices(IWorkerProvider workerProvider)
        => _workerProvider = workerProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_workerProvider.SupportDistributed)
            return;

        if (stoppingToken.IsCancellationRequested)
        {
            await _workerProvider.LogOutAsync();
        }
        else
        {
            while (true)
            {
                await _workerProvider.RefreshAsync();
                await Task.Delay(Const.DEFAULT_HEARTBEATINTERVAL, stoppingToken);
            }
        }
    }
}
