// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Internal;

internal class DefaultAliyunStorageOptionProvider : IAliyunStorageOptionProvider
{
    private AliyunStorageOptions _aliyunStorageOptions;

    public bool SupportCallback { get; private set; }

    public bool IncompleteStsOptions { get; private set; }

    internal DefaultAliyunStorageOptionProvider(AliyunStorageOptions aliyunStorageOptions)
    {
        _aliyunStorageOptions = aliyunStorageOptions;
        Refresh();
    }

    public DefaultAliyunStorageOptionProvider(IOptionsMonitor<AliyunStorageConfigureOptions> options, string name)
        : this(options.Get(name))
    {
        options.OnChange(aliyunStorageOptions =>
        {
            _aliyunStorageOptions = GetAliyunStorageOptions(aliyunStorageOptions);
            Refresh();
        });
    }

    public DefaultAliyunStorageOptionProvider(AliyunStorageConfigureOptions options)
        : this(GetAliyunStorageOptions(options))
    {
        Refresh();
    }

    // public DefaultAliyunStorageOptionProvider(IOptionsMonitor<AliyunStorageConfigureOptions> options)
    //     : this(GetAliyunStorageOptions(options.CurrentValue))
    // {
    //     options.OnChange(aliyunStorageConfigureOptions =>
    //     {
    //         _aliyunStorageOptions = GetAliyunStorageOptions(aliyunStorageConfigureOptions);
    //         Refresh();
    //     });
    // }

    public AliyunStorageOptions GetOptions() => _aliyunStorageOptions;

    private static AliyunStorageOptions GetAliyunStorageOptions(AliyunStorageConfigureOptions options)
    {
        AliyunStorageOptions aliyunStorageOptions = options.Storage;
        aliyunStorageOptions.AccessKeyId = TryUpdate(options.Storage.AccessKeyId, options.AccessKeyId)!;
        aliyunStorageOptions.AccessKeySecret = TryUpdate(options.Storage.AccessKeySecret, options.AccessKeySecret)!;
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

    private static string? TryUpdate(string? source, string? destination)
    {
        if (!string.IsNullOrWhiteSpace(source))
            return source;

        return destination;
    }

    private void Refresh()
    {
        SupportCallback = !string.IsNullOrEmpty(_aliyunStorageOptions.CallbackBody) &&
            !string.IsNullOrEmpty(_aliyunStorageOptions.CallbackUrl);

        IncompleteStsOptions = _aliyunStorageOptions.Sts == null ||
            string.IsNullOrEmpty(_aliyunStorageOptions.Sts.RegionId) ||
            string.IsNullOrEmpty(_aliyunStorageOptions.RoleArn) ||
            string.IsNullOrEmpty(_aliyunStorageOptions.RoleSessionName);
    }
}
