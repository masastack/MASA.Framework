[中](README.zh-CN.md) | EN

## AutoComplete

Example：

```c#
Install-Package Masa.Contrib.SearchEngine.AutoComplete
```

Basic usage:

Using AutoComplete

```` C#

string userIndexName = "user_index_01";
string userAlias = "user_index";
builder.Services
        .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
        .AddAutoComplete(option => option.UseIndexName(userIndexName).UseAlias(userAlias));
````

##### setting data

```` C#
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
````


##### retrieve data

```` C#
public async Task<string> GetAsync([FromServices] IAutoCompleteClient client)
{
     var response = await client.GetAsync<long>("Edward Adam Davis");
     return System.Text.Json.JsonSerializer.Serialize(response);
}
````

> Plugins that need to be added by default:
>
> https://github.com/medcl/elasticsearch-analysis-ik
> https://github.com/medcl/elasticsearch-analysis-pinyin