// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.DistributedMemory.Models;

/// <summary>
/// The MASA memory cache options.
/// </summary>
public class MasaMemoryCacheOptions : MemoryCacheOptions
{
    /// <summary>
    /// Gets or sets the <see cref="SubscribeKeyType"/>.
    /// </summary>
    public SubscribeKeyTypes SubscribeKeyType { get; set; } = SubscribeKeyTypes.ValueTypeFullNameAndKey;

    /// <summary>
    /// Gets or sets the prefix of subscribe key.
    /// </summary>
    public string SubscribeKeyPrefix { get; set; } = string.Empty;
}
