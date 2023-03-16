// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests.Isolation;

public static class ObjectStorageUtils
{
    private static PropertyInfo _aliyunStorageOptionsPropertyInfo =
        typeof(ObjectStorageClientBase).GetProperty("Options", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public static AliyunStorageOptions GetAliyunStorageOptions(object storageClient)
    {
        var options = _aliyunStorageOptionsPropertyInfo.GetValue(storageClient) as AliyunStorageOptions;
        Assert.IsNotNull(options);
        return options;
    }
}
