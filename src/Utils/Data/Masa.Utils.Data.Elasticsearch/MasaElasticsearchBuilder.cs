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
            _elasticClientFactory ??= Services.BuildServiceProvider().GetRequiredService<IElasticClientFactory>();
            if (AlwaysGetNewestElasticClient)
                return _elasticClientFactory.Create(Name);

            return _elasticClient ??= _elasticClientFactory.Create(Name);
        }
    }

    public IMasaElasticClient Client => new DefaultMasaElasticClient(ElasticClient);

    public bool AlwaysGetNewestElasticClient { get; }

    public MasaElasticsearchBuilder(IServiceCollection services, string name, bool alwaysGetNewestElasticClient)
    {
        Services = services;
        Name = name;
        AlwaysGetNewestElasticClient = alwaysGetNewestElasticClient;
    }
}
