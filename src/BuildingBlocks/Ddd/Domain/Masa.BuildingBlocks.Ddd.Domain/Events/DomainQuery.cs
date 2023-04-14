// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public abstract record DomainQuery<TResult>(Guid IntegrationEventId, DateTime IntegrationEvenCreateTime) : IDomainQuery<TResult>
{
    [JsonInclude] public Guid IntegrationEventId { private get; set; } = IntegrationEventId;

    [JsonInclude] public DateTime IntegrationEvenCreateTime { private get; set; } = IntegrationEvenCreateTime;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork
    {
        get => null;
        set => throw new NotSupportedException(nameof(UnitOfWork));
    }

    public abstract TResult Result { get; set; }

    protected DomainQuery() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Guid GetEventId() => IntegrationEventId;

    public void SetEventId(Guid eventId) => IntegrationEventId = eventId;

    public DateTime GetCreationTime() => IntegrationEvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => IntegrationEvenCreateTime = creationTime;
}
