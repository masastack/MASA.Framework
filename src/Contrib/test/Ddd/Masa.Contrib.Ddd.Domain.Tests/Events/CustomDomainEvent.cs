// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record CustomDomainEvent : DomainEvent
{
    public CustomDomainEvent() : base()
    {

    }

    public CustomDomainEvent(Guid eventId, DateTime creationTime) : base(eventId, creationTime)
    {
    }
}
