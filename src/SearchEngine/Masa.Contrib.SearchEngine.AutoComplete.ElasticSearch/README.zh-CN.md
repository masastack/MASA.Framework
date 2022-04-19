中 | [EN](README.md)

## AutoComplete.ElasticSearch

用例：

```c#
Install-Package Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch
```

基本用法:

使用AutoComplete

``` C#

string userIndexName = "user_index_01";
string userAlias = "user_index";
builder.Services
       .AddElasticsearchClient("es", option => option.UseNodes("http://localhost:9200").UseDefault())
       .AddAutoComplete<long>(option => option.UseIndexName(userIndexName).UseAlias(userAlias));
```

##### 设置数据
* SetAsync 设置单个文档
* SetMultiAsync 设置多个文档

1. 设置单个文档:

   ``` C#
   public async Task SetMultiAsync([FromServices] IAutoCompleteClient client)
   {
       await client.SetAsync(new AutoCompleteDocument<long>()
       {
           Text = "Edward Adam Davis",
           Value = 1
       });
   }
   ```

2. 设置多个文档:

   ``` C#
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
   ```

##### 获取数据

* GetAsync

1. 根据关键字搜索:

   ``` C#
   public async Task<string> GetAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.GetAsync<long>("Edward Adam Davis");
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ```

##### 删除文档

* DeleteAsync 删除单个文档
* DeleteMultiAsync 删除多个文档

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
   public async Task<string> DeleteMultiAsync([FromServices] IAutoCompleteClient client)
   {
       var response = await client.DeleteMultiAsync(new long[] { 1, 2 });
       return System.Text.Json.JsonSerializer.Serialize(response);
   }
   ```

默认需要添加的插件：

> https://github.com/medcl/elasticsearch-analysis-ik
>
> https://github.com/medcl/elasticsearch-analysis-pinyin