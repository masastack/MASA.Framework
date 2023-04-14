// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public abstract record IntegrationEvent : IIntegrationEvent
{
    [JsonInclude]public Guid IntegrationEventId { private get; set; }

    [JsonInclude]
    public DateTime IntegrationEvenCreateTime { private get; set; }

    [NotMapped] [JsonIgnore] public IUnitOfWork? UnitOfWork { get; set; }

    public virtual string Topic { get; set; }

    protected IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow)
    {
    }

    protected IntegrationEvent(Guid eventId, DateTime creationTime)
    {
        if (string.IsNullOrWhiteSpace(Topic)) Topic = GetType().Name;

        IntegrationEventId = eventId;
        IntegrationEvenCreateTime = creationTime;
    }

    public Guid GetEventId() => IntegrationEventId;

    public void SetEventId(Guid eventId) => IntegrationEventId = eventId;

    public DateTime GetCreationTime() => IntegrationEvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => IntegrationEvenCreateTime = creationTime;
}
