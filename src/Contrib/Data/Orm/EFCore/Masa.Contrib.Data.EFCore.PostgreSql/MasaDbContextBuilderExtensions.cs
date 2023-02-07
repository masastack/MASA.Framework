// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseNpgsql(
        this MasaDbContextBuilder builder,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseNpgsql(
                connectionStringProvider.GetConnectionString(name),
                npgsqlOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseNpgsql(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        => builder.UseNpgsqlCore(connectionString, false, npgsqlOptionsAction);

    public static MasaDbContextBuilder UseNpgsql(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        => builder.UseNpgsqlCore(connection, false, npgsqlOptionsAction);

    public static MasaDbContextBuilder UseTestNpgsql(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        => builder.UseNpgsqlCore(connection, true, npgsqlOptionsAction);

    public static MasaDbContextBuilder UseTestNpgsql(
        this MasaDbContextBuilder builder,
        string connectionString,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        => builder.UseNpgsqlCore(connectionString, true, npgsqlOptionsAction);

    private static MasaDbContextBuilder UseNpgsqlCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        bool isTest,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseNpgsql(connectionString, npgsqlOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connectionString, isTest);
    }

    private static MasaDbContextBuilder UseNpgsqlCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseNpgsql(connection, npgsqlOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(connection.ConnectionString, isTest);
    }
}
