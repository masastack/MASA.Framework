// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Oracle;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextBuilder UseOracle(
        this MasaDbContextBuilder builder,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseOracle(
                connectionStringProvider.GetConnectionString(name),
                oracleOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseOracle(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
        => builder.UseOracleCore(connectionString, false, oracleOptionsAction);

    public static MasaDbContextBuilder UseTestOracle(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
        => builder.UseOracleCore(connectionString, true, oracleOptionsAction);

    private static MasaDbContextBuilder UseOracleCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        bool isTest,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseOracle(connectionString, oracleOptionsAction);
        return builder.UseOracleCore(connectionString, isTest);
    }

    public static MasaDbContextBuilder UseOracle(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
        => builder.UseOracleCore(connection, false, oracleOptionsAction);

    public static MasaDbContextBuilder UseTestOracle(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
        => builder.UseOracleCore(connection, true, oracleOptionsAction);

    private static MasaDbContextBuilder UseOracleCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest = false,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseOracle(connection, oracleOptionsAction);
        return builder.UseOracleCore(connection.ConnectionString, isTest);
    }

    private static MasaDbContextBuilder UseOracleCore(
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
