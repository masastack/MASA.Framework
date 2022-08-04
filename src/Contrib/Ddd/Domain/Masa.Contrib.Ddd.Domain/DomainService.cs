// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain;

public class DomainService : IDomainService
{
    public IDomainEventBus EventBus { get; }

    public DomainService(IDomainEventBus eventBus) => EventBus = eventBus;
}
