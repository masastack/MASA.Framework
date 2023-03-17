// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch.Tests;

[TestClass]
public class DefaultElasticClientProviderTest
{
    private readonly DefaultElasticClientProvider _defaultElasticClientProvider;

    public DefaultElasticClientProviderTest()
    {
        _defaultElasticClientProvider = new DefaultElasticClientProvider();
    }

    [TestMethod]
    public void TestGetClient()
    {
        var elasticsearchOptions = new ElasticsearchOptions("localhost:9200");
        var item = _defaultElasticClientProvider.GetClient(elasticsearchOptions);

        var elasticsearchOptions2 = new ElasticsearchOptions("localhost:9200");
        var item2 = _defaultElasticClientProvider.GetClient(elasticsearchOptions2);

        Assert.AreEqual(item.ElasticClient, item2.ElasticClient);
        Assert.AreEqual(item.MasaElasticClient, item2.MasaElasticClient);

        var elasticsearchOptions3 = new ElasticsearchOptions("localhost:9201");
        var item3 = _defaultElasticClientProvider.GetClient(elasticsearchOptions3);

        Assert.AreNotEqual(item.ElasticClient, item3.ElasticClient);
        Assert.AreNotEqual(item.MasaElasticClient, item3.MasaElasticClient);
    }
}
