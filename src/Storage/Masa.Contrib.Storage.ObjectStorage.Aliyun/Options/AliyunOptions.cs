// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;

public class AliyunOptions
{
    public string AccessKeyId { get; set; }

    public string AccessKeySecret { get; set; }

    public string RegionId { get; set; }

    private string _endpoint;

    public string Endpoint
    {
        get => _endpoint;
        set => _endpoint = value?.Trim() ?? string.Empty;
    }

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

    public long GetDurationSeconds() => DurationSeconds ?? Const.DEFAULT_DURATION_SECONDS;

    public long GetEarlyExpires() => EarlyExpires ?? Const.DEFAULT_EARLY_EXPIRES;

    internal static string GetRegionId(string endpoint)
    {
        endpoint = endpoint.Trim();
        if (endpoint.EndsWith(Const.INTERNAL_ENDPOINT_SUFFIX, StringComparison.OrdinalIgnoreCase))
            return endpoint.Remove(endpoint.Length - Const.INTERNAL_ENDPOINT_SUFFIX.Length);

        if (endpoint.EndsWith(Const.PUBLIC_ENDPOINT_DOMAIN_SUFFIX, StringComparison.OrdinalIgnoreCase))
            return endpoint.Remove(endpoint.Length - Const.PUBLIC_ENDPOINT_DOMAIN_SUFFIX.Length);

        throw new ArgumentException(Const.ERROR_ENDPOINT_MESSAGE);
    }
}
