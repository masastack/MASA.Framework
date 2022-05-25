// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public abstract class BaseIdGenerator
{
    private readonly IWorkerProvider _workerProvider;

    /// <summary>
    /// initial timestamp
    /// </summary>
    protected readonly long Twepoch;

    private readonly uint _timestampType;

    /// <summary>
    /// sequence mask, used to limit the sequence maximum
    /// </summary>
    protected readonly long SequenceMask;

    protected readonly int SequenceBits;

    /// <summary>
    /// Timestamp shift offset bits
    /// </summary>
    protected readonly int TimestampLeftShift;

    /// <summary>
    /// Sequence within milliseconds
    /// </summary>
    protected long Sequence;

    /// <summary>
    /// The last time the ID was generated
    /// </summary>
    protected long LastTimestamp = -1L;

    protected readonly object Lock = new();

    public BaseIdGenerator(IWorkerProvider workerProvider, IdGeneratorOptions idGeneratorOptions)
    {
        _workerProvider = workerProvider;
        _timestampType = idGeneratorOptions.TimestampType;
        Twepoch = new DateTimeOffset(idGeneratorOptions.BaseTime).ToUnixTimeMilliseconds();
        SequenceMask = ~(-1 << idGeneratorOptions.SequenceBits);
        SequenceBits = idGeneratorOptions.SequenceBits;
        TimestampLeftShift = idGeneratorOptions.SequenceBits + idGeneratorOptions.WorkerIdBits;
    }

    public virtual long Generate()
    {
        lock (Lock)
        {
            var currentTimestamp = GetCurrentTimestamp();

            if (currentTimestamp < LastTimestamp)
            {
                return NextIdByTimeCallback(currentTimestamp, LastTimestamp);
            }

            if (LastTimestamp == currentTimestamp)
            {
                Sequence = (Sequence + 1) & SequenceMask;
                if (Sequence == 0) currentTimestamp = TilNextMillis(LastTimestamp);
            }
            else
            {
                Sequence = 0;
            }

            LastTimestamp = currentTimestamp;

            return NextId(currentTimestamp - Twepoch);
        }
    }

    protected virtual long NextIdByTimeCallback(long currentTimestamp, long lastTimestamp)
    {
        return 0;
    }

    protected virtual long NextId(long deltaSeconds)
    {
        return (deltaSeconds << TimestampLeftShift)
            | (GetWorkerId() << SequenceBits)
            | Sequence;
    }

    protected virtual long TilNextMillis(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp) timestamp = GetCurrentTimestamp();
        return timestamp;
    }

    protected long GetWorkerId()
        => _workerProvider.GetWorkerIdAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    protected long GetCurrentTimestamp() => new DateTimeOffset(DateTime.UtcNow).GetTimestamp(_timestampType);
}
