// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class MasaDbConnectionOptions
{
    private static readonly List<KeyValuePair<string, string>> _connectionStrings = new();
    public ConnectionStrings ConnectionStrings { get; set; }

    public MasaDbConnectionOptions()
    {
        ConnectionStrings = new ConnectionStrings(_connectionStrings);
    }

    public void TryAddConnectionString(string name, string connectionString)
    {
        if (_connectionStrings.All(item => item.Key != name))
            _connectionStrings.Add(new KeyValuePair<string, string>(name, connectionString));
    }
}
