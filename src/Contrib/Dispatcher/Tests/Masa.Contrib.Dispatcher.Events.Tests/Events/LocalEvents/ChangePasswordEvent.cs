// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public class ChangePasswordEvent : IEvent
{
    public string Account { get; set; }

    public string Content { get; set; }

    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    public Guid GetEventId() => Id;

    public void SetEventId(Guid eventId) => Id = eventId;

    public DateTime GetCreationTime() => CreationTime;

    public void SetCreationTime(DateTime creationTime) => CreationTime = creationTime;
}
