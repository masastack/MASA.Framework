// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseInMemoryDatabase(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<InMemoryDbContextOptionsBuilder>? inMemoryOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseInMemoryDatabase(connectionString, inMemoryOptionsAction);
        return builder;
    }
}
