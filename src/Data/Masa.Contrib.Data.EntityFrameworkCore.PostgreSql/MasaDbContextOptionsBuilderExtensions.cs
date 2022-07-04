// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.PostgreSql;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextBuilder UseNpgsql(
        this MasaDbContextBuilder builder,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
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
        return builder.UseNpgsqlCore(connectionString, isTest);
    }

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

    private static MasaDbContextBuilder UseNpgsqlCore(
        this MasaDbContextBuilder builder,
        DbConnection connection,
        bool isTest,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseNpgsql(connection, npgsqlOptionsAction);
        return builder.UseNpgsqlCore(connection.ConnectionString, isTest);
    }

    private static MasaDbContextBuilder UseNpgsqlCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        bool isTest = false)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        if (!isTest && dbConnectionOptions.ConnectionStrings.ContainsKey(name))
            throw new ArgumentException($"The [{builder.DbContextType.Name}] Database Connection String already exists");

        dbConnectionOptions.TryAddConnectionString(name, connectionString);
        return builder;
    }
}
