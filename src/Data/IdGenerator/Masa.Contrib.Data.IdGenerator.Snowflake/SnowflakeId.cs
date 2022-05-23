// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

internal class SnowflakeId
{
    /// <summary>
    /// start time
    /// </summary>
    private static long _twepoch;

    /// <summary>
    /// Machine ID is shifted to the left
    /// </summary>
    private readonly int _workerIdShift;

    /// <summary>
    /// data identification id is shifted to the left
    /// </summary>
    private readonly int _datacenterIdShift;

    /// <summary>
    /// time cut left
    /// </summary>
    private readonly int _timestampLeftShift;

    /// <summary>
    /// Mask of the generated sequence, here 4095 (0b111111111111=0xfff=4095)
    /// </summary>
    private readonly long _sequenceMask;

    /// <summary>
    /// Sequence within milliseconds (0~4095)
    /// </summary>
    private long _sequence;

    /// <summary>
    /// The last time the ID was generated
    /// </summary>
    private long _lastTimestamp = -1L;

    /// <summary>
    /// Work machine ID (0~31)
    /// </summary>
    private readonly long _workerId;

    /// <summary>
    /// Data center ID (0~31)
    /// </summary>
    private readonly long? _datacenterId;

    private readonly object _lock = new();

    public SnowflakeId(IdGeneratorOptions idGeneratorOptions)
    {
        _twepoch = (long)(DateTime.UtcNow - idGeneratorOptions.BaseTime).TotalMilliseconds;
        _workerIdShift = idGeneratorOptions.SequenceBits;
        _datacenterIdShift = idGeneratorOptions.SequenceBits + idGeneratorOptions.WorkerIdBits;
        _timestampLeftShift = idGeneratorOptions.SequenceBits + idGeneratorOptions.WorkerIdBits + idGeneratorOptions.DatacenterIdBits ?? 0;
        _sequenceMask = -1 ^ (-1 << idGeneratorOptions.SequenceBits);
        _workerId = idGeneratorOptions.WorkerId;
        _datacenterId = idGeneratorOptions.DatacenterId;
    }

    public virtual long NextId()
    {
        lock (_lock)
        {
            var timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
                throw new Exception(
                    $"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & _sequenceMask;
                if (_sequence == 0) timestamp = TilNextMillis(_lastTimestamp);
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            if (_datacenterId != null)
            {
                return ((timestamp - _twepoch) << _timestampLeftShift)
                    | (_datacenterId.Value << _datacenterIdShift)
                    | (_workerId << _workerIdShift)
                    | _sequence;
            }

            return ((timestamp - _twepoch) << _timestampLeftShift)
                | _workerId << _workerIdShift
                | _sequence;
        }
    }

    protected virtual long TilNextMillis(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp) timestamp = GetCurrentTimestamp();
        return timestamp;
    }

    protected virtual long GetCurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
