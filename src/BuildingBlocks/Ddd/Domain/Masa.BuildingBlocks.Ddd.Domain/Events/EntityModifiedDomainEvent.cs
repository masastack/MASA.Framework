// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events
{
    public record class EntityModifiedDomainEvent<TEntity> : EntityChangedEvent<TEntity>
    {
        public EntityModifiedDomainEvent(TEntity entity) : base(entity)
        {
        }
    }
}
