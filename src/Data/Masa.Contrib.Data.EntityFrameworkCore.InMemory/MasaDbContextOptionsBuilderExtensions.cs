// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.InMemory;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseInMemoryDatabase(
        this MasaDbContextOptionsBuilder builder,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseInMemoryDatabase(
                connectionStringProvider.GetConnectionString(name),
                inMemoryOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseInMemoryDatabase(
        this MasaDbContextOptionsBuilder builder,
        string databaseName,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
    {
        builder.UseInMemoryDatabaseCore(databaseName);
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseInMemoryDatabase(databaseName, inMemoryOptionsAction);
        return builder;
    }

    private static MasaDbContextOptionsBuilder UseInMemoryDatabaseCore(this MasaDbContextOptionsBuilder builder, string databaseName)
    {
        var dbConnectionOptions = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>().CurrentValue;
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        dbConnectionOptions.TryAddConnectionString(name, databaseName);
        return builder;
    }
}
