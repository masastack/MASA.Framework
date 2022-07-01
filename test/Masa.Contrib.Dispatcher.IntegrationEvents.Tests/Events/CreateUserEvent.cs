// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Events;

public record CreateUserEvent : IEvent
{
    public string Name { get; set; }

    public Guid Id { get; set; }

    public DateTime CreationTime { get; set; }

    public CreateUserEvent()
    {
        this.Id = Guid.NewGuid();
        this.CreationTime = DateTime.UtcNow;
    }

    public CreateUserEvent(string name) : this()
    {
        this.Name = name;
    }

    public Guid GetEventId() => Id;

    public void SetEventId(Guid eventId) => Id = eventId;

    public DateTime GetCreationTime() => CreationTime;

    public void SetCreationTime(DateTime creationTime) => CreationTime = creationTime;
}
