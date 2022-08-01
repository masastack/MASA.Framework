// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record CustomDomainCommand : DomainCommand
{
    public CustomDomainCommand() : base()
    {

    }

    public CustomDomainCommand(Guid eventId, DateTime creationTime) : base(eventId, creationTime)
    {
    }
}
