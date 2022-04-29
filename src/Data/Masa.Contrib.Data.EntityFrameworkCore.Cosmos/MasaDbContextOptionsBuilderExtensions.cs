// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Data;

namespace Masa.Contrib.Data.EntityFrameworkCore.Cosmos;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseCosmos(
        this MasaDbContextOptionsBuilder builder,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        var connectionStringProvider = builder.ServiceProvider.GetRequiredService<IConnectionStringProvider>();
        var configurationDic = connectionStringProvider.GetConnectionString().ToDictionary();

        if (!configurationDic.TryGetValue("Database", out string? databaseName))
            throw new ArgumentException("Cosmos: Bad database connection string, Failed to get [Database] name");

        if (configurationDic.TryGetValue("ConnectionString", out string? connectionString))
            return builder.UseCosmos(connectionString, databaseName, cosmosOptionsAction);

        if (!configurationDic.TryGetValue("AccountKey", out string? accountKey) ||
            !configurationDic.TryGetValue("AccountEndpoint", out string? accountEndpoint))
            throw new ArgumentException("Cosmos: Bad database connection string, Failed to get [AccountKey] name or [AccountEndpoint] name");

        return builder.UseCosmos(accountEndpoint, accountKey, databaseName, cosmosOptionsAction);
    }

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
