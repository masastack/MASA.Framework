// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.DomainServices;

public class CustomDomainService : DomainService
{
    public CustomDomainService(IDomainEventBus domainEventBus) : base(domainEventBus)
    {

    }
}
