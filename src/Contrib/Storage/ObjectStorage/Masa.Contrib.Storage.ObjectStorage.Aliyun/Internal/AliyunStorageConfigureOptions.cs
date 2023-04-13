// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class AliyunStorageConfigureOptions : AliyunOptions
{
    public AliyunStorageOptions Storage { get; set; } = new();

    /// <summary>
    /// Aliyun STS configuration, it is not necessarily a storage configuration
    /// </summary>
    public AliyunStsOptions? Sts { get; set; }

    public static implicit operator AliyunStorageOptions(AliyunStorageConfigureOptions options)
    {
        var aliyunStorageOptions = options.Storage;
        aliyunStorageOptions.AccessKeyId = !options.Storage.AccessKeyId.IsNullOrWhiteSpace() ? options.Storage.AccessKeyId : options.AccessKeyId;
        aliyunStorageOptions.AccessKeySecret = !options.Storage.AccessKeySecret.IsNullOrWhiteSpace() ? options.Storage.AccessKeySecret : options.AccessKeySecret;
        if (options.Storage.Sts != null)
        {
            aliyunStorageOptions.Sts = new AliyunStsOptions(options.Storage.Sts.RegionId)
            {
                DurationSeconds = options.Storage.Sts.DurationSeconds ?? options.Storage.Sts.GetDurationSeconds(),
                EarlyExpires = options.Storage.Sts.EarlyExpires ?? options.Storage.Sts.GetEarlyExpires(),
            };
        }
        else if (options.Sts != null)
        {
            aliyunStorageOptions.Sts = new AliyunStsOptions(options.Sts.RegionId)
            {
                DurationSeconds = options.Sts.DurationSeconds ?? options.Sts.GetDurationSeconds(),
                EarlyExpires = options.Sts.EarlyExpires ?? options.Sts.GetEarlyExpires(),
            };
        }

        return aliyunStorageOptions;
    }
}
