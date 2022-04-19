[中](README.zh-CN.md) | EN

## AutoComplete.ElasticSearch

Example：

```c#
Install-Package Masa.Contrib.SearchEngine.AutoComplete
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

##### Setting data
* SetAsync sets a single document
* SetMultiAsync set multiple documents

1. Set up a single document:

   ```` C#
   public async Task SetMultiAsync([FromServices] IAutoCompleteClient client)
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
   public async Task SetMultiAsync([FromServices] IAutoCompleteClient client)
   {
       await client.SetMultiAsync(new AutoCompleteDocument<long>[]
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

##### Retrieve data

* GetAsync

1. Search by keyword:

   ```` C#
   public async Task<string> GetAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.GetAsync<long>("Edward Adam Davis");
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ````

##### Delete document

* DeleteAsync deletes a single document
* DeleteMultiAsync deletes multiple documents

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
   public async Task<string> DeleteMultiAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.DeleteMultiAsync(new long[] { 1, 2 });
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ````

Plugins that need to be added by default:

> https://github.com/medcl/elasticsearch-analysis-ik
>
> https://github.com/medcl/elasticsearch-analysis-pinyin
