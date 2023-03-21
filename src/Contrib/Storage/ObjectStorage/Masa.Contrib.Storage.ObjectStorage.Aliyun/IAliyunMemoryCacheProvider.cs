// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public interface IAliyunMemoryCacheProvider
{
    IMemoryCache GetMemoryCache(string name, AliyunStorageOptions aliyunStorageOptions);

    void TryRemove(string name, AliyunStorageOptions aliyunStorageOptions);
}
