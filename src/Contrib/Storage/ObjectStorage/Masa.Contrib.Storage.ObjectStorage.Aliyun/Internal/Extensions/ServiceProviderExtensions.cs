// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

internal static class ServiceProviderExtensions
{
    public static IAliyunStorageOptionProvider GetAliyunStorageOptionProvider(
        this IServiceProvider serviceProvider,
        string sectionName,
        string name)
    {
        if (serviceProvider.IsEnabledIsolation(
                sectionName,
                name,
                out AliyunStorageConfigureOptions? aliyunStorageConfigureOptions,
                out IOptionsMonitor<AliyunStorageConfigureOptions>? defaultStorageConfigureOptionsMonitor))
        {
            return new DefaultAliyunStorageOptionProvider(aliyunStorageConfigureOptions);
        }
        return new DefaultAliyunStorageOptionProvider(defaultStorageConfigureOptionsMonitor, name);
    }

    public static bool IsEnabledIsolation(
        this IServiceProvider serviceProvider,
        string sectionName,
        string name,
        [NotNullWhen(true)] out AliyunStorageConfigureOptions? aliyunStorageConfigureOptions,
        [NotNullWhen(false)] out IOptionsMonitor<AliyunStorageConfigureOptions>? defaultStorageConfigureOptionsMonitor)
    {
        var options = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        var isEnable = options.Value.Enable;
        if (isEnable)
        {
            var isolationConfigurationProvider = serviceProvider.GetRequiredService<IIsolationConfigurationProvider>();
            if (!isolationConfigurationProvider.TryGetModule(sectionName, out aliyunStorageConfigureOptions))
            {
                //使用默认配置,增加日志
                aliyunStorageConfigureOptions = serviceProvider.GetDefaultAliyunStorageConfigureOptions(name);
            }
            defaultStorageConfigureOptionsMonitor = null;
        }
        else
        {
            aliyunStorageConfigureOptions = null;
            defaultStorageConfigureOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();
        }
        return isEnable;
    }

    public static AliyunStorageConfigureOptions GetDefaultAliyunStorageConfigureOptions(this IServiceProvider serviceProvider, string name)
    {
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AliyunStorageConfigureOptions>>();
        return optionsMonitor.Get(name);
    }
}
