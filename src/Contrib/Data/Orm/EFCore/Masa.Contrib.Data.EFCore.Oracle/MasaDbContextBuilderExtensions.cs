// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseOracle(
        this MasaDbContextBuilder builder,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
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
        => builder.UseOracleCore(connectionString, oracleOptionsAction);

    public static MasaDbContextBuilder UseOracle(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
        => builder.UseOracleCore(connection, oracleOptionsAction);

    private static MasaDbContextBuilder UseOracleCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseOracle(connectionString, oracleOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connectionString);
    }

    private static MasaDbContextBuilder UseOracleCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseOracle(connection, oracleOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connection.ConnectionString);
    }
}
