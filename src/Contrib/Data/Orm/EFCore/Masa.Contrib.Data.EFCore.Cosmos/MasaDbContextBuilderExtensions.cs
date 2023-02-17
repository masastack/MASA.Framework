// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseCosmos(
        this MasaDbContextBuilder builder,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            var configurationDic = connectionStringProvider.GetConnectionString(name).ToDictionary();

            if (!configurationDic.TryGetValue("Database", out string? databaseName))
                throw new ArgumentException("Cosmos: Bad database connection string, Failed to get [Database] name");

            if (configurationDic.TryGetValue("ConnectionString", out string? connectionString))
            {
                dbContextOptionsBuilder.UseCosmos(connectionString, databaseName, cosmosOptionsAction);
                return;
            }

            if (!configurationDic.TryGetValue("AccountKey", out string? accountKey) ||
                !configurationDic.TryGetValue("AccountEndpoint", out string? accountEndpoint))
                throw new ArgumentException(
                    "Cosmos: Bad database connection string, Failed to get [AccountKey] name or [AccountEndpoint] name");

            dbContextOptionsBuilder.UseCosmos(accountEndpoint, accountKey, databaseName, cosmosOptionsAction);
        };
        return builder;
    }

    public static MasaDbContextBuilder UseCosmos(
        this MasaDbContextBuilder builder,
        string accountEndpoint,
        string accountKey,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
        => builder.UseCosmosCore(accountEndpoint, accountKey, databaseName, cosmosOptionsAction);

    public static MasaDbContextBuilder UseCosmos(
        this MasaDbContextBuilder builder,
        string connectionString,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
        => builder.UseCosmosCore(connectionString, databaseName, cosmosOptionsAction);

    private static MasaDbContextBuilder UseCosmosCore(
        this MasaDbContextBuilder builder,
        string accountEndpoint,
        string accountKey,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseCosmos(accountEndpoint, accountKey, databaseName, cosmosOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations($"AccountEndpoint={accountEndpoint};AccountKey={accountKey};Database={databaseName};");
    }

    private static MasaDbContextBuilder UseCosmosCore(
        this MasaDbContextBuilder builder,
        string connectionString,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseCosmos(connectionString, databaseName, cosmosOptionsAction);
        return builder.ConfigMasaDbContextAndConnectionStringRelations($"{connectionString};Database={databaseName};");
    }
}
