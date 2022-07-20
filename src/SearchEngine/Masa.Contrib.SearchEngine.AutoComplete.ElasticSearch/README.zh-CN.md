中 | [EN](README.md)

## AutoComplete.ElasticSearch

用例：

```c#
Install-Package Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch
```

基本用法:

使用AutoComplete

* 使用默认模型`AutoCompleteDocument<TValue>`，其中TValue仅支持简单类型

``` C#
string userIndexName = "user_index_01";
string userAlias = "user_index";
builder.Services
       .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
       .AddAutoComplete<long>(option => option.UseIndexName(userIndexName).UseAlias(userAlias));
```

* 使用自定义模型，例如: `UserDocument`

``` C#
string userIndexName = "user_index_01";
string userAlias = "user_index";
builder.Services
       .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
       .AddAutoCompleteBySpecifyDocument<UserDocument>(option => option.UseIndexName(userIndexName).UseAlias(userAlias));

public class User : AutoCompleteDocument
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    protected override string GetText()
    {
        return $"{Name}:{Phone}";
    }
}
```

##### 设置数据 (SetAsync)

1. 设置单个文档:

1.1. 使用默认模型（键值模型）:

    ``` C#
    public async Task SetAsync([FromServices] IAutoCompleteClient client)
    {
        await client.SetAsync(new AutoCompleteDocument<long>("Edward Adam Davis", 1));
    }
    ```

1.2 使用自定义模型

    ``` C#
    public async Task SetAsync([FromServices] IAutoCompleteClient client)
    {
        await client.SetBySpecifyDocumentAsync(new User
        {
            Id = 1,
            Name = "托尼",
            Phone = "13999999999"
        });
    }
    ```

2. 设置多个文档:

2.1 使用默认模型（键值模型）:

    ``` C#
    public async Task SetAsync([FromServices] IAutoCompleteClient client)
    {
        await client.SetAsync(new AutoCompleteDocument<long>[]
        {
            new("Edward Adam Davis", 1),
            new("Edward Jim", 1)
        });
    }
    ```

2.2 使用自定义模型

    ``` C#
    public async Task SetAsync([FromServices] IAutoCompleteClient client)
    {
        await client.SetBySpecifyDocumentAsync(new User[]
        {
            new()
            {
                Id = 1,
                Name = "吉姆",
                Phone = "13999999999"
            },
            new()
            {
                Id = 2,
                Name = "托尼",
                Phone = "13888888888"
            }
        });
    }
    ```

##### 获取数据 (GetAsync)

1. 根据关键字搜索:

1.1 使用默认模型（键值模型）:

    ``` C#
    public async Task<string> GetAsync([FromServices] IAutoCompleteClient client)
    {
        var response = await client.GetAsync<long>("Edward Adam Davis");
        return System.Text.Json.JsonSerializer.Serialize(response);
    }
    ```

1.2 使用自定义模型

    ```
    public async Task<string> GetAsync([FromServices] IAutoCompleteClient client)
    {
        var response = await client.GetBySpecifyDocumentAsync<User>("托尼");
        return System.Text.Json.JsonSerializer.Serialize(response);
    }
    ```

##### 删除文档 (DeleteAsync)

1. 删除单个文档:

   ``` C#
   public async Task<string> DeleteAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.DeleteAsync(1);
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ```

2. 删除多个文档:

   ``` C#
   public async Task<string> DeleteAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.DeleteAsync(new long[] { 1, 2 });
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ```

默认需要添加的插件：

> https://github.com/medcl/elasticsearch-analysis-ik
>
> https://github.com/medcl/elasticsearch-analysis-pinyin

## 常见问题

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