// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSpliting.Cqrs.Tests;

public class ProductionQueryHandler : QueryHandler<ProductionItemQuery, string>
{
    public override Task HandleAsync(ProductionItemQuery @event)
    {
        if (string.IsNullOrEmpty(@event.ProductionId))
            throw new ArgumentNullException(nameof(@event));

        if (@event.GetEventId() == default(Guid) || @event.GetCreationTime() > DateTime.UtcNow)
            throw new ArgumentNullException(nameof(@event));

        if (@event.ProductionId == "1")
            @event.Result = "Apple";

        return Task.CompletedTask;
    }
}
