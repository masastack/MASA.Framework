// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities;

public class EntityComparer<TEntity> : IEqualityComparer<Entity>
    where TEntity : IEntity
{
    public bool Equals(Entity? x, Entity? y)
    {
        if (x is null ^ y is null) return false;

        if (x is null) return true;

        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(Entity obj)
    {
        return obj.GetKeys().Select(key => key.Value).Aggregate(0, HashCode.Combine);
    }
}
