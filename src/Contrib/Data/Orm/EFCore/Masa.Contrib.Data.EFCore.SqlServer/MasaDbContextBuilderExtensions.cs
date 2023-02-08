// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseSqlServer(
        this MasaDbContextBuilder builder,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseSqlServer(
                connectionStringProvider.GetConnectionString(name),
                sqlServerOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseSqlServer(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
        => builder.UseSqlServerCore(connectionString, false, sqlServerOptionsAction);

    public static MasaDbContextBuilder UseSqlServer(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
        => builder.UseSqlServerCore(connection, false, sqlServerOptionsAction);

    public static MasaDbContextBuilder UseTestSqlServer(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
        => builder.UseSqlServerCore(connection, true, sqlServerOptionsAction);

    public static MasaDbContextBuilder UseTestSqlServer(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction)
        => builder.UseSqlServerCore(connectionString, true, sqlServerOptionsAction);

    private static MasaDbContextBuilder UseSqlServerCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        bool isTest,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connectionString, isTest);
    }

    private static MasaDbContextBuilder UseSqlServerCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseSqlServer(connection, sqlServerOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connection.ConnectionString, isTest);
    }
}
