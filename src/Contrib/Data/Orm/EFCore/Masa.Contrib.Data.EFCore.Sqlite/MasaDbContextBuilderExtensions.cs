// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseSqlite(
        this MasaDbContextBuilder builder,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
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
        return builder.UseSqliteCore(connectionString, isTest);
    }

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

    private static MasaDbContextBuilder UseSqliteCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseSqlite(connection, sqliteOptionsAction);
        return builder.UseSqliteCore(connection.ConnectionString, isTest);
    }

    private static MasaDbContextBuilder UseSqliteCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        bool isTest = false)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        if (!isTest && dbConnectionOptions.ConnectionStrings.ContainsKey(name))
            throw new ArgumentException($"The [{builder.DbContextType.Name}] Database Connection String already exists");

        dbConnectionOptions.TryAddConnectionString(name, connectionString);
        return builder;
    }
}
