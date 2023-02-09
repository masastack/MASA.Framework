// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class MasaElasticsearchBuilder
{
    public IServiceCollection Services { get; }

    public string Name { get; }

    private IElasticClientFactory? _elasticClientFactory;

    private IElasticClient? _elasticClient;

    public IElasticClient ElasticClient
    {
        get
        {
            if (_elasticClient == null)
            {
                _elasticClientFactory ??= Services.BuildServiceProvider().GetRequiredService<IElasticClientFactory>();
                _elasticClient = _elasticClientFactory.Create(Name);
            }

            return _elasticClient;
        }
    }

    public IMasaElasticClient Client => new DefaultMasaElasticClient(ElasticClient);

    public MasaElasticsearchBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }
}
