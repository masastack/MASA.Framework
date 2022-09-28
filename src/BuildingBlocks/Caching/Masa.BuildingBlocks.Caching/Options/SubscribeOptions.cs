// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class SubscribeOptions<T> : BasePubSubOptions
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Is it a publisher client
    /// </summary>
    public bool IsPublisherClient { get; set; }

    public SubscribeOptions(Guid uniquelyIdentifies) : base(uniquelyIdentifies)
    {
    }
}
