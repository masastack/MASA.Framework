[中](README.zh-CN.md) | EN

## AutoComplete.ElasticSearch

Example：

```c#
Install-Package Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch
```

Basic usage:

Using AutoComplete

```` C#

string userIndexName = "user_index_01";
string userAlias ​​= "user_index";
builder.Services
       .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
       .AddAutoComplete<long>(option => option.UseIndexName(userIndexName).UseAlias(userAlias));
````

##### Setting data (SetAsync)

1. Set up a single document:

   ```` C#
   public async Task SetAsync([FromServices] IAutoCompleteClient client)
   {
       await client.SetAsync(new AutoCompleteDocument<long>()
       {
           Text = "Edward Adam Davis",
           Value = 1
       });
   }
   ````

2. Set up multiple documents:

   ```` C#
   public async Task SetAsync([FromServices] IAutoCompleteClient client)
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

##### Get data (GetAsync)

1. Search by keyword:

   ```` C#
   public async Task<string> GetAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.GetAsync<long>("Edward Adam Davis");
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ````

##### Delete document (DeleteAsync)

1. To delete a single document:

   ```` C#
   public async Task<string> DeleteAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.DeleteAsync(1);
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ````

2. To delete multiple documents:

   ```` C#
   public async Task<string> DeleteAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.DeleteAsync(new long[] { 1, 2 });
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ````

Plugins that need to be added by default:

> https://github.com/medcl/elasticsearch-analysis-ik
>
> https://github.com/medcl/elasticsearch-analysis-pinyin

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