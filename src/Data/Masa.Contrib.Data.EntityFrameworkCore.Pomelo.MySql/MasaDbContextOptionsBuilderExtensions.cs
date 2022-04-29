// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Pomelo.MySql;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseMySql(
        this MasaDbContextOptionsBuilder builder,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        var connectionStringProvider = builder.ServiceProvider.GetRequiredService<IConnectionStringProvider>();
        return builder.UseMySql(connectionStringProvider.GetConnectionString(), serverVersion, mySqlOptionsAction);
    }

    public static MasaDbContextOptionsBuilder UseMySql(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseMySql(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseMySql(connection, serverVersion, mySqlOptionsAction);
        return builder;
    }
}
