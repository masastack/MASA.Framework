// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseMySQL(
        this MasaDbContextBuilder builder,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
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
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connectionString, isTest);
    }

    private static MasaDbContextBuilder UseMySQLCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseMySQL(connection, mySqlOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connection.ConnectionString, isTest);
    }
}
