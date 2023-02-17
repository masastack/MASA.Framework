// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class MasaDbConnectionOptions
{
    public ConnectionStrings ConnectionStrings { get; set; }

    public MasaDbConnectionOptions()
    {
        ConnectionStrings = new ConnectionStrings();
    }

    public void TryAddConnectionString(string name, string connectionString)
    {
        if (ConnectionStrings.All(item => !item.Key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            ConnectionStrings.Add(name, connectionString);
    }
}
