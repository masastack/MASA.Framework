// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

internal class MemoryCacheProvider
{
    private readonly MemoryCache<string, IMemoryCache> _data = new MemoryCache<string, IMemoryCache>();

    public IMemoryCache GetMemoryCache(string name)
        => _data.GetOrAdd(name, _ => new MemoryCache(Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions())));
}
