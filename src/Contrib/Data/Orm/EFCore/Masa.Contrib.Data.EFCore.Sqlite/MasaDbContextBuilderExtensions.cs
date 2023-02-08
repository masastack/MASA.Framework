// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseSqlite(
        this MasaDbContextBuilder builder,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseSqlite(
                connectionStringProvider.GetConnectionString(name),
                sqliteOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseSqlite(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
        => builder.UseSqliteCore(connectionString, false, sqliteOptionsAction);

    public static MasaDbContextBuilder UseSqlite(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
        => builder.UseSqliteCore(connection, false, sqliteOptionsAction);

    public static MasaDbContextBuilder UseTestSqlite(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
        => builder.UseSqliteCore(connection, true, sqliteOptionsAction);

    public static MasaDbContextBuilder UseTestSqlite(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
        => builder.UseSqliteCore(connectionString, true, sqliteOptionsAction);

    private static MasaDbContextBuilder UseSqliteCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        bool isTest,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseSqlite(connectionString, sqliteOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connectionString, isTest);
    }

    private static MasaDbContextBuilder UseSqliteCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseSqlite(connection, sqliteOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connection.ConnectionString, isTest);
    }
}
