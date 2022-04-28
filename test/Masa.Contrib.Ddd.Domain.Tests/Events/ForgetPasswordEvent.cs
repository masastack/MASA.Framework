// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public class ForgetPasswordEvent : IEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    public string Account { get; set; }
}
