// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Caching;

public class CacheKeyAliasOptions
{
    /// <summary>
    /// Refresh TypeAlias minimum interval time
    /// default: 30s
    /// </summary>
    public long RefreshTypeAliasInterval { get; set; } = 30;

    public Func<Dictionary<string, string>> GetAllTypeAliasFunc { get; set; }
}
