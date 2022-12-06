// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class DefaultElasticsearchFactory : IElasticsearchFactory
{
    private readonly IElasticClientFactory _factory;

    public DefaultElasticsearchFactory(IElasticClientFactory factory)
        => _factory = factory;

    public IMasaElasticClient CreateClient() => new DefaultMasaElasticClient(CreateElasticClient());

    public IMasaElasticClient CreateClient(string name) => new DefaultMasaElasticClient(CreateElasticClient(name));

    public IElasticClient CreateElasticClient() => _factory.Create();

    public IElasticClient CreateElasticClient(string name) => _factory.Create(name);
}
