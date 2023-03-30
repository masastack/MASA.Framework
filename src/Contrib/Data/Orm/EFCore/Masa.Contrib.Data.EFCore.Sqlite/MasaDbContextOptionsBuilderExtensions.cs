// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextOptionsBuilderExtensions
{
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
