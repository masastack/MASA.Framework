// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public interface IElasticsearchFactory
{
    IMasaElasticClient CreateClient();

    IMasaElasticClient CreateClient(string name);

    IElasticClient CreateElasticClient();

    IElasticClient CreateElasticClient(string name);
}
