// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Tests;

[TestClass]
public class DefaultMasaElasticClientTests
{
    private MasaElasticsearchBuilder _builder = default!;

    [TestInitialize]
    public void Initialize()
    {
        IServiceCollection service = new ServiceCollection();
        _builder = service.AddElasticsearchClient("es", "http://localhost:9200");
    }

    [TestMethod]
    public async Task TestCreateIndexAsyncReturnIndexIsExist()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        var indexResponse = await _builder.Client.CreateIndexAsync(indexName);
        Assert.IsTrue(indexResponse.IsValid);
        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestIndexExistAsyncReturnIndexIsExist()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        await _builder.Client.CreateIndexAsync(indexName);

        var existResponse = await _builder.Client.IndexExistAsync(indexName);
        Assert.IsTrue(existResponse.IsValid && existResponse.Exists);
        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestCreateDocumentAsyncReturnCountIs1()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(!countResponse.IsValid);

        var createResponse = await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new
        {
            id = Guid.NewGuid()
        }, Guid.NewGuid().ToString()));
        Assert.IsTrue(createResponse.IsValid);

        Thread.Sleep(1000);
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 1);
        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestDeleteMultiIndexAsyncReturnCountIs0()
    {
        string userIndexName = $"user_index_{Guid.NewGuid()}";
        var indexResponse = await _builder.Client.CreateIndexAsync(userIndexName);
        Assert.IsTrue(indexResponse.IsValid);

        string personIndexName = $"person_index_{Guid.NewGuid()}";
        indexResponse = await _builder.Client.CreateIndexAsync(personIndexName);
        Assert.IsTrue(indexResponse.IsValid);

        var response = await _builder.Client.DeleteMultiIndexAsync(new[] { userIndexName, personIndexName });
        Assert.IsTrue(response.IsValid);

        Thread.Sleep(1000);
        Assert.IsTrue(!(await _builder.Client.IndexExistAsync(userIndexName)).Exists &&
            !(await _builder.Client.IndexExistAsync(personIndexName)).Exists);
    }

    [TestMethod]
    public async Task TestIndexByAliasAsync()
    {
        string userIndex1Name = $"user_index_{Guid.NewGuid()}";
        string userIndex2Name = $"user_index_{Guid.NewGuid()}";
        string aliasIndexName = $"user_index_alias_{Guid.NewGuid()}";

        IAliases aliases = new Aliases();
        aliases.Add(aliasIndexName, new Alias());
        var indexResponse = await _builder.Client.CreateIndexAsync(userIndex1Name, new CreateIndexOptions()
        {
            Aliases = aliases
        });
        indexResponse = await _builder.Client.CreateIndexAsync(userIndex2Name, new CreateIndexOptions()
        {
            Aliases = aliases
        });

        Thread.Sleep(1000);

        var getIndexResponse = await _builder.Client.GetAllIndexAsync();
        Assert.IsTrue(getIndexResponse.IsValid && getIndexResponse.IndexNames.Contains(userIndex1Name) &&
            getIndexResponse.IndexNames.Contains(userIndex2Name));

        var getIndexByAliasResponse = await _builder.Client.GetIndexByAliasAsync(aliasIndexName);
        Assert.IsTrue(getIndexByAliasResponse.IsValid && getIndexByAliasResponse.IndexNames.Length == 2 &&
            getIndexByAliasResponse.IndexNames.Contains(userIndex1Name) &&
            getIndexByAliasResponse.IndexNames.Contains(userIndex2Name));

        var deleteIndexResponse = await _builder.Client.DeleteIndexByAliasAsync(aliasIndexName);
        Assert.IsTrue(deleteIndexResponse.IsValid);
    }

    [TestMethod]
    public async Task TestGetAllAliasAsyncReturnAliasCountIs1()
    {
        string userIndexName = $"user_index_{Guid.NewGuid()}";
        string aliasIndexName = $"user_index_alias_{Guid.NewGuid()}";

        var aliasResponse = await _builder.Client.GetAllAliasAsync();
        Assert.IsTrue(aliasResponse.IsValid);

        var oldAliasesCount = aliasResponse.Aliases.Count();
        IAliases aliases = new Aliases();
        aliases.Add(aliasIndexName, new Alias());
        await _builder.Client.CreateIndexAsync(userIndexName, new CreateIndexOptions()
        {
            Aliases = aliases
        });

        Thread.Sleep(1000);
        aliasResponse = await _builder.Client.GetAllAliasAsync();
        Assert.IsTrue(aliasResponse.IsValid && aliasResponse.Aliases.Count() == oldAliasesCount + 1);

        await _builder.Client.DeleteIndexByAliasAsync(aliasIndexName);
    }

    [TestMethod]
    public async Task TestGetAliasByIndexAsyncReturnAliasIsUserIndexAlias()
    {
        string userIndexName = $"user_index_{Guid.NewGuid()}";
        string aliasIndexName = $"user_index_alias_{Guid.NewGuid()}";

        IAliases aliases = new Aliases();
        aliases.Add(aliasIndexName, new Alias());
        await _builder.Client.CreateIndexAsync(userIndexName, new CreateIndexOptions()
        {
            Aliases = aliases
        });

        Thread.Sleep(1000);
        var aliasResponse = await _builder.Client.GetAliasByIndexAsync(userIndexName);
        Assert.IsTrue(aliasResponse.IsValid && aliasResponse.Aliases.Count() == 1 && aliasResponse.Aliases.Contains(aliasIndexName));

        await _builder.Client.DeleteIndexByAliasAsync(aliasIndexName);
    }

    [TestMethod]
    public async Task TestUnBindAliasAsyncReturnIndexIsExist()
    {
        string userIndexName = $"user_index_{Guid.NewGuid()}";
        string aliasIndexName = $"user_index_alias_{Guid.NewGuid()}";

        IAliases aliases = new Aliases();
        aliases.Add(aliasIndexName, new Alias());
        await _builder.Client.CreateIndexAsync(userIndexName, new CreateIndexOptions()
        {
            Aliases = aliases
        });

        Thread.Sleep(1000);

        var bulkAliasResponse = await _builder.Client.UnBindAliasAsync(new UnBindAliasIndexOptions(aliasIndexName, userIndexName));
        Assert.IsTrue(bulkAliasResponse.IsValid);

        Thread.Sleep(1000);
        var existsResponse = await _builder.Client.IndexExistAsync(userIndexName);
        Assert.IsTrue(existsResponse.IsValid && existsResponse.Exists);
    }

    [TestMethod]
    public async Task TestCreateMultiDocumentAsyncReturnCountIs2()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        await _builder.Client.DeleteIndexAsync(indexName);

        string id = Guid.NewGuid().ToString();
        string id2 = Guid.NewGuid().ToString();
        var createMultiResponse = await _builder.Client.CreateMultiDocumentAsync(new CreateMultiDocumentRequest<object>(indexName)
        {
            Items = new List<SingleDocumentBaseRequest<object>>()
            {
                new(new
                {
                    Id = Guid.NewGuid()
                }, id),
                new(new
                {
                    Id = Guid.NewGuid()
                }, id2)
            }
        });
        Assert.IsTrue(createMultiResponse.IsValid &&
            createMultiResponse.Items.Count == 2 &&
            createMultiResponse.Items.Count(r => r.IsValid) == 2);

        Thread.Sleep(1000);
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 2);
        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestDocumentExistsAsyncReturnIsExist()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        var id = Guid.NewGuid();
        var createResponse = await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new
        {
            id = id
        }, id.ToString()));
        Assert.IsTrue(createResponse.IsValid);
        Thread.Sleep(1000);

        var existsResponse = await _builder.Client.DocumentExistsAsync(new ExistDocumentRequest(indexName, id.ToString()));
        Assert.IsTrue(existsResponse.IsValid && existsResponse.Exists);

        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestDeleteDocumentAsyncReturnCountIs0()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        var id = Guid.NewGuid();
        var createResponse = await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new
        {
            id = id
        }, id.ToString()));
        Assert.IsTrue(createResponse.IsValid);

        var deleteResponse = await _builder.Client.DeleteDocumentAsync(new DeleteDocumentRequest(indexName, id.ToString()));
        Assert.IsTrue(deleteResponse.IsValid);

        Thread.Sleep(1000);
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 0);

        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestDeleteMultiDocumentAsyncReturnDeleteCountIs2()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        string id = Guid.NewGuid().ToString();
        string id2 = Guid.NewGuid().ToString();
        await _builder.Client.CreateMultiDocumentAsync(new CreateMultiDocumentRequest<object>(indexName)
        {
            Items = new List<SingleDocumentBaseRequest<object>>()
            {
                new(new
                {
                    Id = Guid.NewGuid()
                }, id),
                new(new
                {
                    Id = Guid.NewGuid()
                }, id2)
            }
        });

        var deleteResponse =
            await _builder.Client.DeleteMultiDocumentAsync(new DeleteMultiDocumentRequest(indexName, id, id2));
        Assert.IsTrue(deleteResponse.IsValid && deleteResponse.Data.Count == 2 && deleteResponse.Data.Count(r => r.IsValid) == 2);

        Thread.Sleep(1000);
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 0);

        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestDeleteMultiDocumentAsyncReturnDeleteCountSuccessIs2()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        string id = Guid.NewGuid().ToString();
        string id2 = Guid.NewGuid().ToString();
        await _builder.Client.CreateMultiDocumentAsync(new CreateMultiDocumentRequest<object>(indexName)
        {
            Items = new List<SingleDocumentBaseRequest<object>>()
            {
                new(new
                {
                    Id = Guid.NewGuid()
                }, id),
                new(new
                {
                    Id = Guid.NewGuid()
                }, id2)
            }
        });

        var deleteResponse =
            await _builder.Client.DeleteMultiDocumentAsync(new DeleteMultiDocumentRequest(indexName, id, id2, Guid.NewGuid().ToString()));
        Assert.IsTrue(deleteResponse.IsValid && deleteResponse.Data.Count == 3 && deleteResponse.Data.Count(r => r.IsValid) == 2);

        Thread.Sleep(1000);
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 0);

        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestSetDocumentAsyncReturnCountIs3()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        Guid id = Guid.NewGuid();
        Guid id2 = Guid.NewGuid();
        await _builder.Client.CreateMultiDocumentAsync(new CreateMultiDocumentRequest<object>(indexName)
        {
            Items = new List<SingleDocumentBaseRequest<object>>()
            {
                new(new
                {
                    Id = id
                }, id.ToString()),
                new(new
                {
                    Id = id2
                }, id2.ToString())
            }
        });
        Guid id3 = Guid.NewGuid();
        var setResponse = await _builder.Client.SetDocumentAsync(new SetDocumentRequest<object>(indexName)
        {
            Items = new List<SingleDocumentBaseRequest<object>>()
            {
                new(new
                {
                    Id = Guid.NewGuid()
                }, id.ToString()),
                new(new
                {
                    Id = id3
                }, id3.ToString())
            }
        });
        Assert.IsTrue(setResponse.IsValid && setResponse.Items.Count == 2 && setResponse.Items.Count(item => item.IsValid) == 2);

        Thread.Sleep(1000);

        var multiResponse = await _builder.Client.GetMultiAsync<object>(
            new GetMultiDocumentRequest(indexName, id.ToString(), id2.ToString(), id3.ToString()));
        Assert.IsTrue(multiResponse.IsValid && multiResponse.Data.Count == 3);
        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestUpdateDocumentAsyncReturnIdEqual1()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        dynamic expandoObject = new ExpandoObject();
        expandoObject.Id = Guid.NewGuid();
        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<ExpandoObject>(indexName, expandoObject, "1"));

        expandoObject.Id = "1";
        var updateDocumentResponse =
            await _builder.Client.UpdateDocumentAsync(new UpdateDocumentRequest<ExpandoObject>(indexName, expandoObject, "1"));
        Assert.IsTrue(updateDocumentResponse.IsValid);

        Thread.Sleep(1000);
        var response = await _builder.Client.GetAsync<ExpandoObject>(new GetDocumentRequest(indexName, "1"));
        Assert.IsTrue(response.IsValid && ((dynamic)(response.Document)).Id == "1");
        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestUpdateMultiDocumentAsyncReturnId1Equeal1AndId2Equal2()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        dynamic expandoObject = new ExpandoObject();
        dynamic expandoObject2 = new ExpandoObject();
        expandoObject.Id = Guid.NewGuid();
        expandoObject2.Id = Guid.NewGuid();
        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<ExpandoObject>(indexName, expandoObject, "1"));
        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<ExpandoObject>(indexName, expandoObject2, "2"));

        expandoObject.Id = "1";
        expandoObject2.Id = "2";
        var updateDocumentResponse =
            await _builder.Client.UpdateMultiDocumentAsync(new UpdateMultiDocumentRequest<ExpandoObject>(indexName)
            {
                Items = new List<UpdateDocumentBaseRequest<ExpandoObject>>()
                {
                    new UpdateDocumentBaseRequest<ExpandoObject>(expandoObject, "1"),
                    new((object)expandoObject2, "2"),
                }
            });
        Assert.IsTrue(updateDocumentResponse.IsValid);

        Thread.Sleep(1000);
        var response = await _builder.Client.GetAsync<ExpandoObject>(new GetDocumentRequest(indexName, "1"));
        Assert.IsTrue(response.IsValid && ((dynamic)(response.Document)).Id == "1");
        response = await _builder.Client.GetAsync<ExpandoObject>(new GetDocumentRequest(indexName, "2"));
        Assert.IsTrue(response.IsValid && ((dynamic)(response.Document)).Id == "2");

        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";

        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new { name = "jim" }, "1"));

        Thread.Sleep(1000);
        var response = await _builder.Client.GetListAsync(new QueryOptions<object>(indexName, "jim", "name", 0, 10));
        Assert.IsTrue(response.IsValid && response.Data.Count == 1);

        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task TestGetPaginatedListAsync()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";

        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new { id = "1", name = "jim" }, "1"));
        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new { id = "2", name = "tom" }, "2"));

        Thread.Sleep(1000);
        var response =
            await _builder.Client.GetPaginatedListAsync(new PaginatedOptions<object>(indexName, "jim", "name", 1, 1));
        Assert.IsTrue(response.IsValid && response.Data.Count == 1);
        response = await _builder.Client.GetPaginatedListAsync(new PaginatedOptions<object>(indexName, "jim or 2",
            new List<string> { "id", "name" }, 1, 2));
        Assert.IsTrue(response.IsValid && response.Data.Count == 2);

        await _builder.Client.DeleteIndexAsync(indexName);
    }

    [TestMethod]
    public async Task BindAliasAsync()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        string indexName2 = $"user_index_{Guid.NewGuid()}";
        string alias = $"userIndex_{Guid.NewGuid()}";

        await _builder.Client.CreateIndexAsync(indexName);
        await _builder.Client.CreateIndexAsync(indexName2);

        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new
        {
            id = Guid.NewGuid()
        }, Guid.NewGuid().ToString()));
        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName2, new
        {
            id = Guid.NewGuid()
        }, Guid.NewGuid().ToString()));

        Thread.Sleep(1000);
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 1);
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName2));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 1);

        await _builder.Client.BindAliasAsync(new BindAliasIndexOptions(alias, new[] { indexName, indexName2 }));
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(alias));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 2);

        await _builder.Client.DeleteIndexAsync(indexName);
        await _builder.Client.DeleteIndexAsync(indexName2);
    }

    [TestMethod]
    public async Task DeleteIndexByAliasAsync()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        string indexName2 = $"user_index_{Guid.NewGuid()}";
        string alias = $"user_index_{Guid.NewGuid()}";

        await _builder.Client.CreateIndexAsync(indexName);
        await _builder.Client.CreateIndexAsync(indexName2);

        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new
        {
            id = Guid.NewGuid()
        }, Guid.NewGuid().ToString()));
        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName2, new
        {
            id = Guid.NewGuid()
        }, Guid.NewGuid().ToString()));

        Thread.Sleep(1000);
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 1);
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName2));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 1);

        await _builder.Client.BindAliasAsync(new BindAliasIndexOptions(alias, new[] { indexName, indexName2 }));
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(alias));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 2);

        await _builder.Client.DeleteIndexByAliasAsync(alias);
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(alias));
        Assert.IsTrue(!countResponse.IsValid);
    }

    [TestMethod]
    public async Task ClearDocumentAsync()
    {
        string indexName = $"user_index_{Guid.NewGuid()}";
        string indexName2 = $"user_index_{Guid.NewGuid()}";
        string alias = $"user_index_{Guid.NewGuid()}";

        await _builder.Client.CreateIndexAsync(indexName);
        await _builder.Client.CreateIndexAsync(indexName2);

        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName, new
        {
            id = Guid.NewGuid()
        }, Guid.NewGuid().ToString()));
        await _builder.Client.CreateDocumentAsync(new CreateDocumentRequest<object>(indexName2, new
        {
            id = Guid.NewGuid()
        }, Guid.NewGuid().ToString()));

        Thread.Sleep(1000);
        var countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 1);
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(indexName2));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 1);

        await _builder.Client.BindAliasAsync(new BindAliasIndexOptions(alias, new[] { indexName, indexName2 }));
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(alias));
        Assert.IsTrue(countResponse.IsValid && countResponse.Count == 2);

        await _builder.Client.ClearDocumentAsync(alias);
        countResponse = await _builder.Client.DocumentCountAsync(new CountDocumentRequest(alias));
        Assert.IsTrue(countResponse.IsValid);

        await _builder.Client.DeleteIndexByAliasAsync(alias);
    }

    [TestMethod]
    public async Task TestAsync()
    {
        string userIndexName = $"user_index";

        IServiceCollection service = new ServiceCollection();
        var builder = service.AddElasticsearchClient("es", "http://localhost:9200");
        await builder.Client.DeleteIndexAsync(userIndexName);
        var serviceProvider = builder.Services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<IMasaElasticClient>();

        var list = new AutoCompleteDocument<long>[]
        {
            new()
            {
                Text = "999999999@qq.com",
                Value = 1
            }
        };
        var request = new SetDocumentRequest<AutoCompleteDocument<long>>(userIndexName);
        foreach (var document in list)
            request.AddDocument(document, document.Id);

        var response = await client.SetDocumentAsync(request, default);
        Assert.IsTrue(response.IsValid);
        await builder.Client.DeleteIndexAsync(userIndexName);
    }
}
