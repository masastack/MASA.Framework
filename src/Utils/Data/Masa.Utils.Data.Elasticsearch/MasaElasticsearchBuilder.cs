// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class MasaElasticsearchBuilder
{
    public IServiceCollection Services { get; }

    public string Name { get; }

    private IElasticClient? _elasticClient;

    public IElasticClient ElasticClient
    {
        get
        {
            if (IsSupportUpdate) throw new NotSupportedException();

            return _elasticClient ??= Services.BuildServiceProvider().GetRequiredService<IElasticClientFactory>().Create(Name);
        }
    }

    public IMasaElasticClient Client => new DefaultMasaElasticClient(ElasticClient);

    public bool IsSupportUpdate { get; }

    public MasaElasticsearchBuilder(IServiceCollection services, string name, bool isSupportUpdate)
    {
        Services = services;
        Name = name;
        IsSupportUpdate = isSupportUpdate;
    }
}
