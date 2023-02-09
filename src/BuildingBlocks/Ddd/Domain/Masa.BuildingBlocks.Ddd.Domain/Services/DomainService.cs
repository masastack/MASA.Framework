// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Ddd.Domain.Services;

public class DomainService : IDomainService
{
    public IDomainEventBus EventBus { get; private set; }

    public DomainService()
    {
        EventBus = null!;
    }

    public DomainService(IDomainEventBus eventBus) => EventBus = eventBus;

    public void SetDomainEventBus(IDomainEventBus domainEventBus)
    {
        EventBus = domainEventBus;
    }
}
