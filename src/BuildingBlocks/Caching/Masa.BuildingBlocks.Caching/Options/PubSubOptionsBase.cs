// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public abstract class PubSubOptionsBase
{
    /// <summary>
    /// Gets or sets the operation.
    /// </summary>
    public SubscribeOperation Operation { get; set; }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// Unique identifier, used to confirm whether the sender and the subscriber are the same client
    /// </summary>
    public Guid UniquelyIdentifies { get; private set; }

    protected PubSubOptionsBase(Guid uniquelyIdentifies)
        => UniquelyIdentifies = uniquelyIdentifies;
}
