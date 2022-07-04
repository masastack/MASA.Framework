// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Entities.Tests.DomainEvents;

/// <summary>
/// todo: It doesn't make sense to send events just for the test loop
/// </summary>
public record UpdateUserDomainEvent : DomainEvent
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}
