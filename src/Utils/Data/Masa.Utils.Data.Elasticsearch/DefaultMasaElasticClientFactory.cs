// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class DefaultMasaElasticClientFactory : IMasaElasticClientFactory
{
    private readonly IElasticClientFactory _clientFactory;

    public DefaultMasaElasticClientFactory(IElasticClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IMasaElasticClient Create()
    {
        var elasticClient = _clientFactory.Create();
        return new DefaultMasaElasticClient(elasticClient);
    }

    public IMasaElasticClient Create(string name)
    {
        var elasticClient = _clientFactory.Create(name);
        return new DefaultMasaElasticClient(elasticClient);
    }
}
