// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.ReadWriteSpliting.Cqrs.Queries;

public abstract record Query<TResult> : IQuery<TResult>
{
    private Guid _eventId;
    private DateTime _creationTime;

    public abstract TResult Result { get; set; }

    protected Query() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    protected Query(Guid eventId, DateTime creationTime)
    {
        _eventId = eventId;
        _creationTime = creationTime;
    }

    public Guid GetEventId() => _eventId;

    public void SetEventId(Guid eventId) => _eventId = eventId;

    public DateTime GetCreationTime() => _creationTime;

    public void SetCreationTime(DateTime creationTime) => _creationTime = creationTime;
}
