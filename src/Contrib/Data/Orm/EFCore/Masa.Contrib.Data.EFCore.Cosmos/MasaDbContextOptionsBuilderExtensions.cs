// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseCosmos(
        this MasaDbContextOptionsBuilder builder,
        string accountEndpoint,
        string accountKey,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseCosmos(accountEndpoint, accountKey, databaseName, cosmosOptionsAction);
        return builder;
    }

    public static MasaDbContextOptionsBuilder UseCosmos(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseCosmos(connectionString, databaseName, cosmosOptionsAction);
        return builder;
    }
}
