// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;

public class AliyunStsOptions
{
    /// <summary>
    /// sts region id
    /// If the RegionId is missing, the temporary Sts credential cannot be obtained.
    /// https://help.aliyun.com/document_detail/371859.html
    /// https://www.alibabacloud.com/help/en/resource-access-management/latest/endpoints#reference-sdg-3pv-xdb
    /// </summary>
    public string? RegionId { get; set; }

    private long? _durationSeconds = null;

    /// <summary>
    /// Set the validity period of the temporary access credential, the minimum is 900, and the maximum is 43200.
    /// default: 3600
    /// unit: second
    /// </summary>
    public long? DurationSeconds
    {
        get => _durationSeconds;
        set
        {
            if (value < 900 || value > 43200)
                throw new ArgumentOutOfRangeException(nameof(DurationSeconds), $"{nameof(DurationSeconds)} must be in range of 900-43200");

            _durationSeconds = value;
        }
    }

    private long? _earlyExpires = null;

    /// <summary>
    /// Voucher expires early
    /// default: 10
    /// unit: second
    /// </summary>
    public long? EarlyExpires
    {
        get => _earlyExpires;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(EarlyExpires), $"{nameof(EarlyExpires)} must be Greater than 0");

            _earlyExpires = value;
        }
    }

    public AliyunStsOptions(string? regionId = null)
    {
        RegionId = regionId;
    }

    public long GetDurationSeconds() => DurationSeconds ?? Const.DEFAULT_DURATION_SECONDS;

    public long GetEarlyExpires() => EarlyExpires ?? Const.DEFAULT_EARLY_EXPIRES;
}
