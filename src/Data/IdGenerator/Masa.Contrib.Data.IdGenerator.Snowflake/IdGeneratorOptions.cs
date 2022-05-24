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
    /// Number of working machines
    /// </summary>
    public int WorkerIdBits { get; set; } = 10;

    /// <summary>
    /// When the machine clock is enabled, the timestamp is meaningless
    /// After the machine clock is enabled, the timestamp will be meaningless.
    /// The time when the project first obtains the id is used as the starting time, which is not affected by the clock callback.
    /// </summary>
    public bool EnableMachineClock { get; set; } = false;

    /// <summary>
    /// Whether to support distributed id
    /// </summary>
    public bool SupportDistributed { get; protected set; } = false;

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
}
