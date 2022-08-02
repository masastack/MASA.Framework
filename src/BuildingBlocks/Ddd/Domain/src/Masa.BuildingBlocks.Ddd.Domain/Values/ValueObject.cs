// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Values;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityValues();

    public override bool Equals(object? obj)
    {
        if (this is null ^ obj is null) return false;

        if (obj is ValueObject entity)
        {
            return entity.GetEqualityValues().SequenceEqual(GetEqualityValues());
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return GetEqualityValues().Aggregate(0, (hashCode, next) => HashCode.Combine(hashCode, next));
    }

    public static bool operator ==(ValueObject x, ValueObject y)
    {
        return x.Equals(y);
    }

    public static bool operator !=(ValueObject x, ValueObject y)
    {
        return !x.Equals(y);
    }
}
