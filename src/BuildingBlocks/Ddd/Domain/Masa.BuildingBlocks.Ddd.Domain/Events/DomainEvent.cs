// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public abstract record DomainEvent(Guid IntegrationEventId, DateTime IntegrationEvenCreateTime) : IDomainEvent
{
    [JsonInclude] public Guid IntegrationEventId { private get; set; } = IntegrationEventId;

    [JsonInclude] public DateTime IntegrationEvenCreateTime { private get; set; } = IntegrationEvenCreateTime;

    [NotMapped]
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Guid GetEventId() => IntegrationEventId;

    public void SetEventId(Guid eventId) => IntegrationEventId = eventId;

    public DateTime GetCreationTime() => IntegrationEvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => IntegrationEvenCreateTime = creationTime;
}
