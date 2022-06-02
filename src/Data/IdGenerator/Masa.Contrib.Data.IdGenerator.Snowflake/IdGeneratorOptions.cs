// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public class IdGeneratorOptions
{
    /// <summary>
    /// Baseline time, it is not recommended to change after use to avoid duplicate ids
    /// </summary>
    public DateTime BaseTime { get; set; } = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// The number of digits the sequence occupies in the id
    /// default: 10
    /// </summary>
    public int SequenceBits { get; set; } = 12;

    /// <summary>
    /// The number of digits occupied by the working machines in the id
    /// </summary>
    public int WorkerIdBits { get; set; } = 10;

    /// <summary>
    /// milliseconds: 1
    /// seconds: 2
    /// </summary>
    public uint TimestampType { get; set; } = 1;

    /// <summary>
    /// When the machine clock is enabled, the timestamp is meaningless
    /// After the machine clock is enabled, the timestamp will be meaningless.
    /// The time when the project first obtains the id is used as the starting time, which is not affected by the clock callback.
    /// </summary>
    public bool EnableMachineClock { get; set; } = false;

    /// <summary>
    /// Maximum acceptable callback duration, default 3000ms(3s)
    /// </summary>
    public long MaxCallBackTime { get; set; } = 3000;

    /// <summary>
    /// Whether to support distributed id
    /// </summary>
    public bool SupportDistributed { get; private set; }

    /// <summary>
    /// WorkerId check interval(Suitable for distributed deployment)
    /// </summary>
    public int HeartbeatInterval { get; set; } = Const.DEFAULT_HEARTBEAT_INTERVAL;

    /// <summary>
    /// The maximum expiration time. When the new WorkerId is not obtained after this time, the WorkerId of the current service will be cancelled.
    /// </summary>
    public int MaxExpirationTime { get; set; } = Const.DEFAULT_EXPIRATION_TIME;

    /// <summary>
    /// Maximum supported worker machine id
    /// </summary>
    public long MaxWorkerId => ~(-1L << WorkerIdBits);

    public void EnableSupportDistributed() => SupportDistributed = true;
}
