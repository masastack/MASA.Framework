// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class PublishOptions : PubSubOptionsBase
{
    public object? Value { get; set; }

    public PublishOptions(Guid uniquelyIdentifies) : base(uniquelyIdentifies)
    {
    }
}
