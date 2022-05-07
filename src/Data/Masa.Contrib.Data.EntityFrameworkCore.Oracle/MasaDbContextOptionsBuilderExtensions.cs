// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Oracle;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseOracle(
        this MasaDbContextOptionsBuilder builder,
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

    public static MasaDbContextOptionsBuilder UseOracle(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        builder.UseOracleCore(connectionString);
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseOracle(connectionString, oracleOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseOracle(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        builder.UseOracleCore(connection.ConnectionString);
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseOracle(connection, oracleOptionsAction);
        return builder;
    }

    private static MasaDbContextOptionsBuilder UseOracleCore(this MasaDbContextOptionsBuilder builder, string connectionString)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        dbConnectionOptions.TryAddConnectionString(name, connectionString);
        return builder;
    }
}
