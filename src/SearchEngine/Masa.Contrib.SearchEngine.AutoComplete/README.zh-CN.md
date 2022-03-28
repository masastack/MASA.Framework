中 | [EN](README.md)

## AutoComplete

用例：

```c#
Install-Package Masa.Contrib.SearchEngine.AutoComplete
```

基本用法:

使用AutoComplete

``` C#

string userIndexName = "user_index_01";
string userAlias = "user_index";
builder.Services
       .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
       .AddAutoComplete(option => option.UseIndexName(userIndexName).UseAlias(userAlias));
```

##### 设置数据

``` C#
public async Task<string> SetAsync([FromServices] IAutoCompleteClient client)
{
    await client.SetAsync(new AutoCompleteDocument<long>[]
    {
        new()
        {
            Text = "Edward Adam Davis",
            Value = 1
        },
        new()
        {
            Text = "Edward Jim",
            Value = 1
        }
    });
}
```


##### 获取数据

``` C#
public async Task<string> GetAsync([FromServices] IAutoCompleteClient client)
{
    var response = await client.GetAsync<long>("Edward Adam Davis");
    return System.Text.Json.JsonSerializer.Serialize(response);
}
```

> 默认需要添加的插件：
>
> https://github.com/medcl/elasticsearch-analysis-ik
>
> https://github.com/medcl/elasticsearch-analysis-pinyin