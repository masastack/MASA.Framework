// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests.Isolation;

public static class ObjectStorageUtils
{
    private static FieldInfo _aliyunStorageOptionsFieldInfo =
        typeof(DefaultStorageClient).GetField("AliyunStorageOptions", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public static AliyunStorageOptions GetAliyunStorageOptions(object storageClient)
    {
        var options = _aliyunStorageOptionsFieldInfo.GetValue(storageClient) as AliyunStorageOptions;
        Assert.IsNotNull(options);
        return options;
    }
}
