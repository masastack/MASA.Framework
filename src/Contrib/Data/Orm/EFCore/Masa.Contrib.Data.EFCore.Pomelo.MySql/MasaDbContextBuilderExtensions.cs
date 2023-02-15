// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseMySql(
        this MasaDbContextBuilder builder,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
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
        => builder.UseMySqlCore(connectionString, serverVersion, mySqlOptionsAction);

    public static MasaDbContextBuilder UseMySql(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
        => builder.UseMySqlCore(connection, serverVersion, mySqlOptionsAction);

    private static MasaDbContextBuilder UseMySqlCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connectionString);
    }

    private static MasaDbContextBuilder UseMySqlCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseMySql(connection, serverVersion, mySqlOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connection.ConnectionString);
    }
}
