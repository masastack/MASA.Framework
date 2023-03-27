// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch;

public class DefaultElasticClientProvider : IElasticClientProvider
{
    private readonly MemoryCache<string, (IElasticClient ElasticClient, IMasaElasticClient MasaElasticClient)> _data = new();

    public (IElasticClient ElasticClient, IMasaElasticClient MasaElasticClient) GetClient(ElasticsearchOptions elasticsearchOptions)
    {
        return _data.GetOrAdd(ConvertToKey(elasticsearchOptions), _ =>
        {
            var elasticClient = ElasticClientUtils.Create(elasticsearchOptions);
            return (elasticClient, new DefaultMasaElasticClient(elasticClient));
        });
    }

    public void TryRemove(ElasticsearchOptions elasticsearchOptions) => _data.Remove(ConvertToKey(elasticsearchOptions));

    private static string ConvertToKey(ElasticsearchOptions elasticsearchOptions)
        => System.Text.Json.JsonSerializer.Serialize(elasticsearchOptions);
}
