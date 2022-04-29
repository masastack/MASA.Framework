// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Sqlite;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseSqlite(
        this MasaDbContextOptionsBuilder builder,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        var connectionStringProvider = builder.ServiceProvider.GetRequiredService<IConnectionStringProvider>();
        return builder.UseSqlite(connectionStringProvider.GetConnectionString(), sqliteOptionsAction);
    }

    public static MasaDbContextOptionsBuilder UseSqlite(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseSqlite(connectionString, sqliteOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseSqlite(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseSqlite(connection, sqliteOptionsAction);
        return builder;
    }
}
