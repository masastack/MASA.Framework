// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public interface IElasticsearchFactory
{
    [Obsolete("Please use IMasaElasticClientFactory.Create() instead")]
    IMasaElasticClient CreateClient();

    [Obsolete("Please use IMasaElasticClientFactory.Create(name) instead")]
    IMasaElasticClient CreateClient(string name);

    [Obsolete("Please use IElasticClientFactory.Create() instead")]
    IElasticClient CreateElasticClient();

    [Obsolete("Please use IElasticClientFactory.Create(name) instead")]
    IElasticClient CreateElasticClient(string name);
}
