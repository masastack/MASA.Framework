// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public class Properties : IEquatable<Properties>, IEquatable<object>
{
    private readonly Dictionary<string, string> _dict;

    public Properties() => _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public Properties(IDictionary<string, string>? dictionary) =>
        _dict = dictionary == null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(dictionary, StringComparer.OrdinalIgnoreCase);

    public Properties(Properties source) => _dict = source._dict;

    public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) => _dict.TryGetValue(key, out value);

    public string? GetProperty(string key)
    {
        _dict.TryGetValue(key, out var result);

        return result;
    }

    public ISet<string> GetPropertyNames() => new HashSet<string>(_dict.Keys);

    public override bool Equals(object? obj)
    {
        if (this is null ^ obj is null) return false;

        if (obj is Properties other)
        {
            return Equals(other);
        }
        else
        {
            return false;
        }
    }

    public bool Equals(Properties? newProperties)
    {
        if (newProperties == null) return false;

        return GetHashCode() == newProperties.GetHashCode();
    }

    public static bool operator ==(Properties? x, Properties? y)
    {
        if (x is null ^ y is null) return false;

        if (x is null) return true;

        return x.Equals(y);
    }

    public static bool operator !=(Properties? x, Properties? y)
    {
        if (x is null ^ y is null) return false;

        if (x is null) return false;

        return !x.Equals(y);
    }

    public override int GetHashCode()
    {
        return _dict.Select(key => key.Key + key.Value).Aggregate(0, (hashCode, next) => HashCode.Combine(hashCode, next));
    }
}
