// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Cosmos;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseCosmos(
        this MasaDbContextOptionsBuilder builder,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        builder.Builder = (serviceProvider, dbContextOptionsBuilder) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);
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

    public static MasaDbContextOptionsBuilder UseCosmos(
        this MasaDbContextOptionsBuilder builder,
        string accountEndpoint,
        string accountKey,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
        => builder.UseCosmosCore(accountEndpoint, accountKey, databaseName, false, cosmosOptionsAction);

    public static MasaDbContextOptionsBuilder UseTestCosmos(
        this MasaDbContextOptionsBuilder builder,
        string accountEndpoint,
        string accountKey,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
        => builder.UseCosmosCore(accountEndpoint, accountKey, databaseName, true, cosmosOptionsAction);

    private static MasaDbContextOptionsBuilder UseCosmosCore(
        this MasaDbContextOptionsBuilder builder,
        string accountEndpoint,
        string accountKey,
        string databaseName,
        bool isTest,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseCosmos(accountEndpoint, accountKey, databaseName, cosmosOptionsAction);
        return builder.UseCosmosCore($"AccountEndpoint={accountEndpoint};AccountKey={accountKey};Database={databaseName};");
    }

    public static MasaDbContextOptionsBuilder UseCosmos(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
        => builder.UseCosmosCore(connectionString, databaseName, false, cosmosOptionsAction);

    public static MasaDbContextOptionsBuilder UseTestCosmos(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        string databaseName,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
        => builder.UseCosmosCore(connectionString, databaseName, true, cosmosOptionsAction);

    private static MasaDbContextOptionsBuilder UseCosmosCore(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        string databaseName,
        bool isTest,
        Action<CosmosDbContextOptionsBuilder>? cosmosOptionsAction = null)
    {
        builder.Builder = (_, dbContextOptionsBuilder)
            => dbContextOptionsBuilder.UseCosmos(connectionString, databaseName, cosmosOptionsAction);
        return builder.UseCosmosCore($"{connectionString};Database={databaseName};", isTest);
    }

    private static MasaDbContextOptionsBuilder UseCosmosCore(
        this MasaDbContextOptionsBuilder builder,
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
