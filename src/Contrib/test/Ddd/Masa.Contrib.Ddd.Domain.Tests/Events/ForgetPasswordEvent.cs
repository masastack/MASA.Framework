// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public class ForgetPasswordEvent : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    public string Account { get; set; }

    public Guid GetEventId() => Id;

    public void SetEventId(Guid eventId) => Id = eventId;

    public DateTime GetCreationTime() => CreationTime;

    public void SetCreationTime(DateTime creationTime) => CreationTime = creationTime;
}
