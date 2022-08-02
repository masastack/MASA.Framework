// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities;

public abstract class AggregateRoot : Entity, IAggregateRoot, IGenerateDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public virtual void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public virtual void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public virtual IEnumerable<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public async Task PublishDomainEventsAsync(IDomainEventBus eventBus)
    {
        while (_domainEvents.Any())
        {
            await eventBus.PublishAsync(_domainEvents.First());
            _domainEvents.RemoveAt(0);
        }
    }
}

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IGenerateDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public AggregateRoot() : base()
    {
    }

    public AggregateRoot(TKey id) : base(id)
    {
    }

    public virtual void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public virtual void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public virtual IEnumerable<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public async Task PublishDomainEventsAsync(IDomainEventBus eventBus)
    {
        while (_domainEvents.Any())
        {
            await eventBus.PublishAsync(_domainEvents.First());
            _domainEvents.RemoveAt(0);
        }
    }
}
