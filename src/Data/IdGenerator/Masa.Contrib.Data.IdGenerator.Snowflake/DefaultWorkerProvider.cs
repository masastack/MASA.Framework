// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public sealed class DefaultWorkerProvider : IWorkerProvider
{
    public bool SupportDistributed => false;

    private readonly long _workerId;

    public DefaultWorkerProvider()
    {
        _workerId = EnironmentExtensions.GetEnvironmentVariable(Const.DEFAULT_WORKER_ID_KEY) ??
            throw new ArgumentNullException(nameof(IdGeneratorOptions.WorkerId));
    }

    public Task<long> GetWorkerIdAsync() => Task.FromResult(_workerId);

    public Task RefreshAsync() => Task.CompletedTask;

    public Task LogOutAsync() => Task.CompletedTask;
}
