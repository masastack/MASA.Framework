﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Entities.Tests.Events;

public record RegisterUserEvent : DomainEvent
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsSendDomainEvent { get; set; } = true;
}
