// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseInMemoryDatabase(
        this MasaDbContextBuilder builder,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            dbContextOptionsBuilder.UseInMemoryDatabase(
                connectionStringProvider.GetConnectionString(name),
                inMemoryOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseInMemoryDatabase(
        this MasaDbContextBuilder builder,
        string databaseName,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
        => builder.UseInMemoryDatabaseCore(databaseName, false, inMemoryOptionsAction);

    public static MasaDbContextBuilder UseInMemoryTestDatabase(
        this MasaDbContextBuilder builder,
        string databaseName,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
        => builder.UseInMemoryDatabaseCore(databaseName, true, inMemoryOptionsAction);

    private static MasaDbContextBuilder UseInMemoryDatabaseCore(
        this MasaDbContextBuilder builder,
        string databaseName,
        bool isTest,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseInMemoryDatabase(databaseName, inMemoryOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations(databaseName, isTest);
    }
}
