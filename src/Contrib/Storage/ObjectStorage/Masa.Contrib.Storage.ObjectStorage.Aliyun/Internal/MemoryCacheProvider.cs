// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

internal class MemoryCacheProvider : IAliyunMemoryCacheProvider
{
    private readonly MemoryCache<string, IMemoryCache> _data = new();

    public IMemoryCache GetMemoryCache(AliyunStorageOptions aliyunStorageOptions)
        => _data.GetOrAdd(ConvertToKey(aliyunStorageOptions),
            _ => new MemoryCache(Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions())));

    public void TryRemove(AliyunStorageOptions aliyunStorageOptions)
        => _data.Remove(ConvertToKey(aliyunStorageOptions));

    private static string ConvertToKey(AliyunStorageOptions aliyunStorageOptions)
        => System.Text.Json.JsonSerializer.Serialize(aliyunStorageOptions);
}
