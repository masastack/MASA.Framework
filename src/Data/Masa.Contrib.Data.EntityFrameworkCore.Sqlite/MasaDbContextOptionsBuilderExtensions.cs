// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Sqlite;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseSqlite(
        this MasaDbContextOptionsBuilder builder,
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

    public static MasaDbContextOptionsBuilder UseSqlite(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        builder.UseSqliteCore(connectionString);
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseSqlite(connectionString, sqliteOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseSqlite(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
    {
        builder.UseSqliteCore(connection.ConnectionString);
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseSqlite(connection, sqliteOptionsAction);
        return builder;
    }

    private static MasaDbContextOptionsBuilder UseSqliteCore(this MasaDbContextOptionsBuilder builder, string connectionString)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        dbConnectionOptions.TryAddConnectionString(name, connectionString);
        return builder;
    }
}
