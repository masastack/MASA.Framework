// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface ICachingBuilder
{
    /// <summary>
    /// Gets the application service collection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Get the name of IDistributedCacheClient or IMultilevelCacheClient, used for multiple IDistributedCacheClient or IMultilevelCacheClient.
    /// </summary>
    string Name { get; }
}
