// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Framework.IntegrationTests.EventBus.Application.Events;

public record AddGoodsIntegrationEvent : IntegrationEvent
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Count { get; set; }

    public decimal Price { get; set; }

    public override string Topic { get; set; } = nameof(AddGoodsIntegrationEvent);
}

public record class TestBaseEventLocal : IntegrationEvent
{
    public TestBaseEventLocal()
    {
    }

    public TestBaseEventLocal(long index)
    {
        Index = index;
    }

    public long Index { get; set; }

    /// <summary>
    /// Topic
    /// </summary>
    public override string Topic { get; set; } = nameof(TestBaseEventLocal);
}
