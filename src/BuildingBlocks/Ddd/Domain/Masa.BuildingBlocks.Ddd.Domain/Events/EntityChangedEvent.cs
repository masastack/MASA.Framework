// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events
{
    public abstract record class EntityChangedEvent<TEntity> : DomainCommand
    {
        public TEntity Entity { get; set; }

        public EntityChangedEvent(TEntity entity)
        {
            Entity = entity;
        }
    }
}
