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

    public static MasaDbContextOptionsBuilder UseMySql(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.UseMySqlCore(connectionString);
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseMySql(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.UseMySqlCore(connection.ConnectionString);
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseMySql(connection, serverVersion, mySqlOptionsAction);
        return builder;
    }

    private static MasaDbContextOptionsBuilder UseMySqlCore(this MasaDbContextOptionsBuilder builder, string connectionString)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        dbConnectionOptions.TryAddConnectionString(name, connectionString);
        return builder;
    }
}
