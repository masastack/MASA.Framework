// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.SqlServer;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseSqlServer(
        this MasaDbContextOptionsBuilder builder,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseSqlServer(
                connectionStringProvider.GetConnectionString(name),
                sqlServerOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseSqlServer(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        builder.UseSqlServerCore(connectionString);
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseSqlServer(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction = null)
    {
        builder.UseSqlServerCore(connection.ConnectionString);
        builder.Builder = (_, dbContextOptionsBuilder) => dbContextOptionsBuilder.UseSqlServer(connection, sqlServerOptionsAction);
        return builder;
    }

    private static MasaDbContextOptionsBuilder UseSqlServerCore(this MasaDbContextOptionsBuilder builder, string connectionString)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        dbConnectionOptions.TryAddConnectionString(name, connectionString);
        return builder;
    }
}
