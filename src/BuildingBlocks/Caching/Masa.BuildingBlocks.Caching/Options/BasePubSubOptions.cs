// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class BasePubSubOptions
{
    /// <summary>
    /// Gets or sets the operation.
    /// </summary>
    public SubscribeOperation Operation { get; set; }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; set; } = default!;
}
