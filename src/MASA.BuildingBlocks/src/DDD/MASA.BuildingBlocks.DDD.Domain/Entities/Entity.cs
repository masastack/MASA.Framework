namespace MASA.BuildingBlocks.DDD.Domain.Entities;

public abstract class Entity : IEntity, IEquatable<Entity>, IEquatable<object>
{
    public abstract IEnumerable<(string Name, object Value)> GetKeys();

    /// <inheritdoc/>
    public override string ToString()
    {
        var keys = GetKeys().ToArray();
        string connector = keys.Length > 1 ? Environment.NewLine : string.Empty;

        return $"{GetType().Name}:{connector}{string.Join(Environment.NewLine, keys.Select(key => $"{key.Name}={key.Value}"))}";
    }

    public override bool Equals(object? obj)
    {
        if (this is null ^ obj is null) return false;

        if (obj is Entity other)
        {
            return other.GetKeys().Select(key => key.Value).SequenceEqual(GetKeys().Select(key => key.Value));
        }
        else
        {
            return false;
        }
    }

    public bool Equals(Entity? other)
    {
        if (this is null ^ other is null) return false;

        return other!.GetKeys().Select(key => key.Value).SequenceEqual(GetKeys().Select(key => key.Value));
    }

    public override int GetHashCode()
    {
        return GetKeys().Select(key => key.Value).Aggregate(0, (hashCode, next) => HashCode.Combine(hashCode, next));
    }

    public static bool operator ==(Entity? x, Entity? y)
    {
        if (x is null ^ y is null) return false;

        if (x is null) return true;

        return x.Equals(y);
    }

    public static bool operator !=(Entity? x, Entity? y)
    {
        if (x is null ^ y is null) return true;

        if (x is null) return false;

        return !x.Equals(y);
    }
}

public class Entity<TKey> : Entity, IEntity<TKey>
{
    public TKey Id { get; set; } = default!;

    public override IEnumerable<(string Name, object Value)> GetKeys()
    {
        yield return ("Id", Id!);
    }
}
