// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Oracle;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseOracle(
        this MasaDbContextOptionsBuilder builder,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        var connectionStringProvider = builder.ServiceProvider.GetRequiredService<IConnectionStringProvider>();
        return builder.UseOracle(connectionStringProvider.GetConnectionString(), oracleOptionsAction);
    }

    public static MasaDbContextOptionsBuilder UseOracle(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseOracle(connectionString, oracleOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseOracle(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<OracleDbContextOptionsBuilder>? oracleOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseOracle(connection, oracleOptionsAction);
        return builder;
    }
}
