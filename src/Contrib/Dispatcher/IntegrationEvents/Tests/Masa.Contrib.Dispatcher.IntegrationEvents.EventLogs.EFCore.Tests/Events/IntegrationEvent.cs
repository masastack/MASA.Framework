// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore.Tests.Events;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }

    public DateTime CreationTime { get; set; }

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public abstract string Topic { get; set; }

    public Dictionary<string, string> Headers { get; set; } = new();

    public IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public IntegrationEvent(Guid Id, DateTime CreationTime)
    {
        this.Id = Id;
        this.CreationTime = CreationTime;
    }

    public Guid GetEventId() => Id;

    public void SetEventId(Guid eventId) => Id = eventId;

    public DateTime GetCreationTime() => CreationTime;

    public void SetCreationTime(DateTime creationTime) => CreationTime = creationTime;
}
