// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities;

public abstract class Entity : IEntity, IEquatable<Entity>, IEquatable<object>
{
    private static readonly EntityComparer<Entity> Instance = new();

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
        if (obj is Entity other) return Instance.Equals(this, other);

        return false;
    }

    public bool Equals(Entity? other) => Instance.Equals(this, other);

    public override int GetHashCode() => Instance.GetHashCode(this);

    public static bool operator ==(Entity? x, Entity? y) => Instance.Equals(x, y);

    public static bool operator !=(Entity? x, Entity? y) => !Instance.Equals(x, y);
}

public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    public TKey Id { get; protected set; } = default!;

    protected Entity()
    {
    }

    protected Entity(TKey id) : this() => Id = id;

    public override IEnumerable<(string Name, object Value)> GetKeys()
    {
        yield return ("Id", Id!);
    }
}
