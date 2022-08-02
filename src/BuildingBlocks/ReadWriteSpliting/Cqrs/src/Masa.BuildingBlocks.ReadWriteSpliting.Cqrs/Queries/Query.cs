// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.ReadWriteSpliting.Cqrs.Queries;

public abstract record Query<TResult> : IQuery<TResult>
    where TResult : notnull
{
    private Guid _eventId;
    private DateTime _creationTime;

    public abstract TResult Result { get; set; }

    public Query() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Query(Guid eventId, DateTime creationTime)
    {
        _eventId = eventId;
        _creationTime = creationTime;
    }

    public Guid GetEventId() => _eventId;

    public void SetEventId(Guid eventId) => _eventId = eventId;

    public DateTime GetCreationTime() => _creationTime;

    public void SetCreationTime(DateTime creationTime) => _creationTime = creationTime;
}
