// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public class DefaultIdGenerator : BaseIdGenerator, IIdGenerator
{
    private readonly ITimeCallbackProvider _timeCallbackProvider;

    public DefaultIdGenerator(
        ITimeCallbackProvider timeCallbackProvider,
        IWorkerProvider workerProvider,
        IdGeneratorOptions idGeneratorOptions)
        : base(workerProvider, idGeneratorOptions)
    {
        _timeCallbackProvider = timeCallbackProvider;
    }

    protected override long NextIdByTimeCallback(long currentTimestamp, long lastTimestamp)
        => _timeCallbackProvider.NextId(currentTimestamp, lastTimestamp, TimestampLeftShift, GetWorkerId(), SequenceBits);
}
