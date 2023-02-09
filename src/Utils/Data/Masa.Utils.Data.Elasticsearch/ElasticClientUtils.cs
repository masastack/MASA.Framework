// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public static class ElasticClientUtils
{
    public static IElasticClient Create(ElasticsearchOptions elasticsearchOptions)
    {
        var settings = elasticsearchOptions.UseConnectionPool
            ? GetConnectionSettingsConnectionPool(elasticsearchOptions)
            : GetConnectionSettingsBySingleNode(elasticsearchOptions);
        return new ElasticClient(settings);
    }

    private static ConnectionSettings GetConnectionSettingsBySingleNode(ElasticsearchOptions relation)
    {
        ArgumentNullException.ThrowIfNull(relation.Nodes);

        var connectionSetting = new ConnectionSettings(new Uri(relation.Nodes.First()))
            .EnableApiVersioningHeader();
        relation.Action?.Invoke(connectionSetting);
        return connectionSetting;
    }

    private static ConnectionSettings GetConnectionSettingsConnectionPool(ElasticsearchOptions relation)
    {
        var pool = new StaticConnectionPool(
            relation.Nodes.Select(node => new Uri(node)),
            relation.StaticConnectionPoolOptions.Randomize,
            relation.StaticConnectionPoolOptions.DateTimeProvider);

        var settings = new ConnectionSettings(
                pool,
                relation.ConnectionSettingsOptions.Connection,
                relation.ConnectionSettingsOptions.SourceSerializerFactory,
                relation.ConnectionSettingsOptions.PropertyMappingProvider)
            .EnableApiVersioningHeader();

        relation.Action?.Invoke(settings);
        return settings;
    }
}
