// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseMySql(
        this MasaDbContextBuilder builder,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseMySql(
                connectionStringProvider.GetConnectionString(name),
                serverVersion,
                mySqlOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseMySql(
        this MasaDbContextBuilder builder,
        string connectionString,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySqlCore(connectionString, serverVersion, false, mySqlOptionsAction);

    public static MasaDbContextBuilder UseTestMySql(
        this MasaDbContextBuilder builder,
        string connectionString,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySqlCore(connectionString, serverVersion, true, mySqlOptionsAction);

    private static MasaDbContextBuilder UseMySqlCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        ServerVersion serverVersion,
        bool isTest,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptionsAction);
        return builder.UseMySqlCore(connectionString, isTest);
    }

    public static MasaDbContextBuilder UseMySql(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySqlCore(connection, serverVersion, false, mySqlOptionsAction);

    public static MasaDbContextBuilder UseTestMySql(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySqlCore(connection, serverVersion, true, mySqlOptionsAction);

    private static MasaDbContextBuilder UseMySqlCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        ServerVersion serverVersion,
        bool isTest,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseMySql(connection, serverVersion, mySqlOptionsAction);
        return builder.UseMySqlCore(connection.ConnectionString, isTest);
    }

    private static MasaDbContextBuilder UseMySqlCore(
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
