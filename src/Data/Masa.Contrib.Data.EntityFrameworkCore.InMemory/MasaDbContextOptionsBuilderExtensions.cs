// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.InMemory;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseInMemoryDatabase(
        this MasaDbContextOptionsBuilder builder,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
    {
        var connectionStringProvider = builder.ServiceProvider.GetRequiredService<IConnectionStringProvider>();
        return builder.UseInMemoryDatabase(connectionStringProvider.GetConnectionString(), inMemoryOptionsAction);
    }

    public static MasaDbContextOptionsBuilder UseInMemoryDatabase(
        this MasaDbContextOptionsBuilder builder,
        string databaseName,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseInMemoryDatabase(databaseName, inMemoryOptionsAction);
        return builder;
    }
}
