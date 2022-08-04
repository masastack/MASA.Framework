// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore.Tests;

public class TestBase
{
    protected readonly string _connectionString = "DataSource=:memory:";
    protected readonly SqliteConnection Connection;

    protected TestBase()
    {
        Connection = new SqliteConnection(_connectionString);
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Close();
    }
}
