// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

internal static class ServiceProviderExtensions
{
    public static AliyunStorageOptions GetAliyunStorageOptions(
        this IServiceProvider serviceProvider,
        string sectionName,
        string name)
    {
        return ComponentConfigUtils.GetComponentConfigByExecute(serviceProvider, name,
            sectionName,
            () => serviceProvider.GetRequiredService<IOptionsSnapshot<AliyunStorageConfigureOptions>>().Get(name));
    }
}
