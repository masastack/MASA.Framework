// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class MasaDbContextConfigurationOptions
{
    private readonly MemoryCache<string, string> _data = new();

    public bool TryGetConnectionString(string name, [NotNullWhen(true)] out string? connectionString)
        => _data.TryGet(name, out connectionString);

    public void AddConnectionString(string name, string connectionString)
        => _data.TryAdd(name, _ => connectionString);
}
