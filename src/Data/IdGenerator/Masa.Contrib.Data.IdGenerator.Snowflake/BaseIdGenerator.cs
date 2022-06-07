// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public abstract class BaseIdGenerator
{
    private readonly IWorkerProvider _workerProvider;

    /// <summary>
    /// start timestamp
    /// </summary>
    protected long Twepoch { get; }

    /// <summary>
    /// milliseconds: 1
    /// seconds: 2
    /// </summary>
    protected TimestampType TimestampType { get; }

    protected long MaxCallBackTime { get; }

    /// <summary>
    /// sequence mask, used to limit the sequence maximum
    /// </summary>
    protected long SequenceMask { get; }

    protected int SequenceBits { get; }

    /// <summary>
    /// Timestamp shift offset bits
    /// </summary>
    protected int TimestampLeftShift { get; }

    /// <summary>
    /// Sequence within milliseconds
    /// </summary>
    protected long Sequence { get; set; }

    /// <summary>
    /// The last time the ID was generated
    /// </summary>
    protected long LastTimestamp { get; set; } = -1L;

    protected object Lock { get; } = new();

    public BaseIdGenerator(IWorkerProvider workerProvider, IdGeneratorOptions idGeneratorOptions)
    {
        _workerProvider = workerProvider;
        TimestampType = idGeneratorOptions.TimestampType;
        MaxCallBackTime = idGeneratorOptions.MaxCallBackTime;
        Twepoch = new DateTimeOffset(idGeneratorOptions.BaseTime).ToUnixTimeMilliseconds();
        SequenceMask = ~(-1 << idGeneratorOptions.SequenceBits);
        SequenceBits = idGeneratorOptions.SequenceBits;
        TimestampLeftShift = idGeneratorOptions.SequenceBits + idGeneratorOptions.WorkerIdBits;
    }

    public virtual long Create()
    {
        lock (Lock)
        {
            var currentTimestamp = GetCurrentTimestamp();

            if (currentTimestamp < LastTimestamp)
            {
                var res = TimeCallBack(currentTimestamp);

                if (res.Support) LastTimestamp = res.LastTimestamp;
                else
                    throw new Exception(
                        $"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {LastTimestamp - currentTimestamp} milliseconds");
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

    protected virtual (bool Support, long LastTimestamp) TimeCallBack(long currentTimestamp) => (false, 0);

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

    protected virtual long GetWorkerId() => _workerProvider.GetWorkerIdAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    protected long GetCurrentTimestamp() => new DateTimeOffset(DateTime.UtcNow).GetTimestamp(TimestampType);
}
