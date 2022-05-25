// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.SqlServer;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseSqlServer(
        this MasaDbContextOptionsBuilder builder,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseSqlServer(
                connectionStringProvider.GetConnectionString(name),
                sqlServerOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseSqlServer(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
        => builder.UseSqlServerCore(connectionString, false, sqlServerOptionsAction);

    public static MasaDbContextOptionsBuilder UseTestSqlServer(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction)
        => builder.UseSqlServerCore(connectionString, true, sqlServerOptionsAction);

    private static MasaDbContextOptionsBuilder UseSqlServerCore(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        bool isTest,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction);
        return builder.UseSqlServerCore(connectionString, isTest);
    }

    public static MasaDbContextOptionsBuilder UseSqlServer(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
        => builder.UseSqlServerCore(connection, false, sqlServerOptionsAction);

    public static MasaDbContextOptionsBuilder UseTestSqlServer(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
        => builder.UseSqlServerCore(connection, true, sqlServerOptionsAction);

    private static MasaDbContextOptionsBuilder UseSqlServerCore(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseSqlServer(connection, sqlServerOptionsAction);
        return builder.UseSqlServerCore(connection.ConnectionString, isTest);
    }

    private static MasaDbContextOptionsBuilder UseSqlServerCore(
        this MasaDbContextOptionsBuilder builder,
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
