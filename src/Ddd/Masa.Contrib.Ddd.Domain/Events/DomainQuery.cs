// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Events;

public abstract record DomainQuery<TResult> : IDomainQuery<TResult>
    where TResult : notnull
{
    private Guid _eventId;
    private DateTime _creationTime;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork
    {
        get => null;
        set => throw new NotSupportedException(nameof(UnitOfWork));
    }

    public abstract TResult Result { get; set; }

    public DomainQuery() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainQuery(Guid eventId, DateTime creationTime)
    {
        _eventId = eventId;
        _creationTime = creationTime;
    }

    public Guid GetEventId() => _eventId;

    public void SetEventId(Guid eventId) => _eventId = eventId;

    public DateTime GetCreationTime() => _creationTime;

    public void SetCreationTime(DateTime creationTime) => _creationTime = creationTime;
}
