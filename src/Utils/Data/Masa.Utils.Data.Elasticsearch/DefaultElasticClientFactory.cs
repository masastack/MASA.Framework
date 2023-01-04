// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class DefaultElasticClientFactory : IElasticClientFactory
{
    private readonly List<string> _names;
    private readonly IOptionsSnapshot<ElasticsearchOptions> _elasticsearchOptions;

    public DefaultElasticClientFactory(IOptionsSnapshot<ElasticsearchOptions> elasticsearchOptions,
        IEnumerable<IConfigureOptions<ElasticsearchOptions>> options)
    {
        _elasticsearchOptions = elasticsearchOptions;
        _names = options.Select(opt => ((ConfigureNamedOptions<ElasticsearchOptions>)opt).Name).ToList();
    }

    public IElasticClient Create()
    {
        var elasticsearchOptions = _elasticsearchOptions.Get(Microsoft.Extensions.Options.Options.DefaultName);

        if (elasticsearchOptions.Nodes is null) elasticsearchOptions = _elasticsearchOptions.Get(_names.FirstOrDefault());

        if (elasticsearchOptions.Nodes is null) throw new ArgumentException("The default ElasticClient is not found, please check if Elasticsearch is added");

        return Create(elasticsearchOptions);
    }

    public IElasticClient Create(string name)
    {
        var elasticsearchOptions = _elasticsearchOptions.Get(name);
        if (elasticsearchOptions.Nodes is null) throw new NotSupportedException($"The ElasticClient whose name is {name} is not found");

        return Create(elasticsearchOptions);
    }

    private static IElasticClient Create(ElasticsearchOptions elasticsearchOptions)
    {
        var settings = elasticsearchOptions.UseConnectionPool
            ? GetConnectionSettingsConnectionPool(elasticsearchOptions)
            : GetConnectionSettingsBySingleNode(elasticsearchOptions);

        return new ElasticClient(settings);
    }

    private static ConnectionSettings GetConnectionSettingsBySingleNode(ElasticsearchOptions relation)
    {
        var connectionSetting = new ConnectionSettings(new Uri(relation.Nodes[0]))
            .EnableApiVersioningHeader();
        relation.Action?.Invoke(connectionSetting);
        return connectionSetting;
    }

    private static ConnectionSettings GetConnectionSettingsConnectionPool(ElasticsearchOptions relation)
    {
        var pool = new StaticConnectionPool(
            relation.Nodes.Select(node => new Uri(node)),
            relation.StaticConnectionPoolOptions?.Randomize ?? true,
            relation.StaticConnectionPoolOptions?.DateTimeProvider);

        var settings = new ConnectionSettings(
                pool,
                relation.ConnectionSettingsOptions?.Connection,
                relation.ConnectionSettingsOptions?.SourceSerializerFactory,
                relation.ConnectionSettingsOptions?.PropertyMappingProvider)
            .EnableApiVersioningHeader();

        relation.Action?.Invoke(settings);
        return settings;
    }
}
