// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseMySQL(
        this MasaDbContextBuilder builder,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseMySQL(
                connectionStringProvider.GetConnectionString(name),
                mySqlOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseMySQL(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySQLCore(connectionString, false, mySqlOptionsAction);

    public static MasaDbContextBuilder UseTestMySQL(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySQLCore(connectionString, true, mySqlOptionsAction);

    private static MasaDbContextBuilder UseMySQLCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        bool isTest,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseMySQL(connectionString, mySqlOptionsAction);
        return builder.UseMySQLCore(connectionString, isTest);
    }

    public static MasaDbContextBuilder UseMySQL(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySQLCore(connection, false, mySqlOptionsAction);

    public static MasaDbContextBuilder UseTestMySQL(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySQLCore(connection, true, mySqlOptionsAction);

    private static MasaDbContextBuilder UseMySQLCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseMySQL(connection, mySqlOptionsAction);
        return builder.UseMySQLCore(connection.ConnectionString, isTest);
    }

    private static MasaDbContextBuilder UseMySQLCore(
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
