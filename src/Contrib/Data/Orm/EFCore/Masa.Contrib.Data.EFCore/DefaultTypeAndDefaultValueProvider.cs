// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class DefaultTypeAndDefaultValueProvider : ITypeAndDefaultValueProvider
{
    private readonly MemoryCache<Type, string?> _typeAndDefaultValues;

    public DefaultTypeAndDefaultValueProvider()
    {
        _typeAndDefaultValues = new();
        _typeAndDefaultValues.AddOrUpdate(typeof(string), _ => null);
    }

    public bool TryAdd(Type type)
    {
        return _typeAndDefaultValues.TryAdd(type, t => Activator.CreateInstance(t)?.ToString());
    }

    public bool TryGet(Type type, [NotNullWhen(true)] out string? defaultValue)
    {
        return _typeAndDefaultValues.TryGet(type, out defaultValue);
    }
}
