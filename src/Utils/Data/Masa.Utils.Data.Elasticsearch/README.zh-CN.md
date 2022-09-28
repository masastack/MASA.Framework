中 | [EN](README.md)

## Masa.Utils.Data.Elasticsearch

用例:

``` powershell
Install-Package Masa.Utils.Data.Elasticsearch
```

### 入门

1. 注册`Elasticsearch`

``` C#
builder.Services.AddElasticsearch("es", "http://localhost:9200"); // 或者builder.Services.AddElasticsearchClient("es", "http://localhost:9200");
```

### 进阶

#### 创建索引：

``` C#
public async Task<string> CreateIndexAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    await client.CreateIndexAsync(indexName);
}
```

#### 删除索引：

``` C#
public async Task<string> DeleteIndexAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    await client.DeleteIndexAsync(indexName);
}
```

#### 根据别名删除索引：

``` C#
public async Task<string> DeleteIndexByAliasAsync([FromServices] IMasaElasticClient client)
{
    string alias = "userIndex";
    await client.DeleteIndexByAliasAsync(alias);
}
```

#### 绑定别名

``` C#
public async Task<string> BindAliasAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    string indexName2 = "user_index_2";
    string alias = "userIndex";
    await client.BindAliasAsync(new BindAliasIndexOptions(alias, new[] { indexName, indexName2 });
}
```

#### 解除别名绑定

``` C#
public async Task<string> BindAliasAsync([FromServices] IMasaElasticClient client)
{
    string indexName = "user_index_1";
    string indexName2 = "user_index_2";
    string alias = "userIndex";
    await client.UnBindAliasAsync(new UnBindAliasIndexOptions(alias, new[] { indexName, indexName2 }));
}
```

> 更多方法请查看[IMasaElasticClient](./IMasaElasticClient.cs)

### 常见问题

1. 出错提示为：`"Content-Type header [application/vnd.elasticsearch+json; compatible-with=7] is not supported"`

    我们默认启用兼容模式，即`EnableApiVersioningHeader(true)`，这样对8.*版本支持很好，但在部分7.*会导致错误，此时需要手动关闭兼容模式，即`EnableApiVersioningHeader(false)`。

    ``` C#
    service.AddElasticsearchClient("es", option =>
    {
        option.UseNodes("http://localhost:9200")
            .UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
    });
    ```

[为何开启兼容模式？](https://github.com/elastic/elasticsearch-net/issues/6154)