// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Services;

public class UserDomainService : DomainService
{
    public UserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task<string> RegisterUserSucceededAsync(RegisterUserSucceededDomainIntegrationEvent domainIntegrationEvent)
    {
        // TODO Simulate a successful message for registered users

        await EventBus.PublishAsync(domainIntegrationEvent);
        return "succeed";
    }
}
