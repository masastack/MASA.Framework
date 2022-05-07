// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.MySql;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseMySQL(
        this MasaDbContextOptionsBuilder builder,
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

    public static MasaDbContextOptionsBuilder UseMySQL(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.UseMySQLCore(connectionString);
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseMySQL(connectionString, mySqlOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseMySQL(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.UseMySQLCore(connection.ConnectionString);
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseMySQL(connection, mySqlOptionsAction);
        return builder;
    }

    private static MasaDbContextOptionsBuilder UseMySQLCore(this MasaDbContextOptionsBuilder builder, string connectionString)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        dbConnectionOptions.TryAddConnectionString(name, connectionString);
        return builder;
    }
}
