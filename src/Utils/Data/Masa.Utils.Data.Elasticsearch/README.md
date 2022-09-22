[中](README.zh-CN.md) | EN

## Masa.Utils.Data.Elasticsearch

Example:

```c#
Install-Package Masa.Utils.Data.Elasticsearch
```

#### Basic usage:

Using Elasticsearch

```` C#
builder.Services.AddElasticsearch("es", "http://localhost:9200"); // or builder.Services.AddElasticsearchClient("es", "http://localhost:9200");
````

#### Create index:

```` C#
public async Task<string> CreateIndexAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    await client.CreateIndexAsync(indexName);
}
````

#### Delete index:

```` C#
public async Task<string> DeleteIndexAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    await client.DeleteIndexAsync(indexName);
}
````

#### Remove indexes based on aliases:

```` C#
public async Task<string> DeleteIndexByAliasAsync([FromServices] IMasaElasticClient client)
{
    string alias = "userIndex";
    await client.DeleteIndexByAliasAsync(alias);
}
````

### bind alias

```` C#
public async Task<string> BindAliasAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    string indexName2 = "user_index_2";
    string alias = "userIndex";
    await client.BindAliasAsync(new BindAliasIndexOptions(alias, new[] { indexName, indexName2 });
}
````

### Unbind aliases

```` C#
public async Task<string> BindAliasAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    string indexName2 = "user_index_2";
    string alias = "userIndex";
    await client.UnBindAliasAsync(new UnBindAliasIndexOptions(alias, new[] { indexName, indexName2 }));
}
````

> For more methods, please see [IMasaElasticClient](./IMasaElasticClient.cs)

## FAQ

1. The error message is: `"Content-Type header [application/vnd.elasticsearch+json; compatible-with=7] is not supported"`

   We enable the compatibility mode by default, namely `EnableApiVersioningHeader(true)`, which supports the 8.* version very well, but will cause errors in some 7.*, in this case, you need to manually turn off the compatibility mode, that is, `EnableApiVersioningHeader(false)`.

     ```` C#
     service.AddElasticsearchClient("es", option =>
     {
         option.UseNodes("http://localhost:9200")
             .UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
     });
     ````

[Why turn on compatibility mode? ](https://github.com/elastic/elasticsearch-net/issues/6154)