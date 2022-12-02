// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public abstract record IntegrationEvent : IIntegrationEvent
{
    private Guid _eventId;
    private DateTime _creationTime;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    [JsonIgnore]
    public virtual string Topic { get; set; }

    protected IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    protected IntegrationEvent(Guid eventId, DateTime creationTime)
    {
        if (string.IsNullOrWhiteSpace(Topic)) Topic = GetType().Name;

        _eventId = eventId;
        _creationTime = creationTime;
    }

    public Guid GetEventId() => _eventId;

    public void SetEventId(Guid eventId) => _eventId = eventId;

    public DateTime GetCreationTime() => _creationTime;

    public void SetCreationTime(DateTime creationTime) => _creationTime = creationTime;
}
