// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public class IdGeneratorOptions
{
    public DateTime BaseTime { get; set; } = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// The number of digits occupied by the machine id
    /// default: 5
    /// </summary>
    public int WorkerIdBits { get; set; } = 5;

    /// <summary>
    /// The number of bits occupied by the data center
    /// </summary>
    public int? DatacenterIdBits { get; set; }

    /// <summary>
    /// The number of digits the sequence occupies in the id
    /// default: 12
    /// </summary>
    public int SequenceBits { get; set; } = 12;

    /// <summary>
    /// Work machine ID (0~31)
    /// </summary>
    public int WorkerId { get; set; }

    /// <summary>
    /// Data center ID (0~31)
    /// </summary>
    public int? DatacenterId { get; set; }

    public string DatacenterIdBitsEnvironmentVariable { get; set; } = Const.DEFAULT_DatacenterIdBits;

    public string WorkerIdEnvironmentVariable { get; set; } = Const.DEFAULT_WORKERID;

    public string DatacenterIdEnvironmentVariable { get; set; } = Const.DEFAULT_DatacenterIdBits;

    /// <summary>
    /// The largest supported machine id, the result is 31 (this shift algorithm can quickly calculate the largest decimal number that can be represented by a few binary digits)
    /// </summary>
    internal long MaxWorkerId => -1L ^ (-1L << WorkerIdBits);

    /// <summary>
    /// The maximum supported data identifier id, the result is 31
    /// </summary>
    internal long? MaxDatacenterId => -1L ^ (-1L << DatacenterIdBits);

    public IdGeneratorOptions()
    {
        DatacenterIdBits = EnironmentExtensions.GetEnvironmentVariable(DatacenterIdBitsEnvironmentVariable);
        WorkerId = EnironmentExtensions.GetEnvironmentVariable(WorkerIdEnvironmentVariable) ?? 0;
        DatacenterId = EnironmentExtensions.GetEnvironmentVariable(DatacenterIdEnvironmentVariable);
    }
}
