// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

internal class ConnectionStringConfigProvider : IConnectionStringConfigProvider
{
    public static readonly MemoryCache<Type, string> ConnectionStrings = new();

    private readonly IOptionsSnapshot<ConnectionStrings> _optionsSnapshot;

    private Dictionary<string, string>? _dictionary;

    public ConnectionStringConfigProvider(IOptionsSnapshot<ConnectionStrings> optionsSnapshot)
    {
        _optionsSnapshot = optionsSnapshot;
    }

    public Dictionary<string, string> GetConnectionStrings()
    {
        if (_dictionary == null)
        {
            _dictionary = new Dictionary<string, string>();

            foreach (var dbContextType in ConnectionStrings.Keys)
            {
                _dictionary.Add(ConnectionStringNameAttribute.GetConnStringName(dbContextType), ConnectionStrings[dbContextType]!);
            }

            foreach (var item in _optionsSnapshot.Value)
                _dictionary.TryAdd(item.Key, item.Value);
        }

        return _dictionary;
    }
}
