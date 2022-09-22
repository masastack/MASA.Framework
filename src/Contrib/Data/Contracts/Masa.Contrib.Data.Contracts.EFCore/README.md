[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.Contracts.EFCore

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.Sqlite
Install-Package Masa.Contrib.Data.Contracts.EFCore //Use the data filtering and soft delete capabilities provided by the protocol
```

### Usage 1

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=test.db;"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering
    optionsBuilder.UseSqlite(); //Using Sqlite database
});
```

3. Use data filtering

``` C#
public async Task<string> GetAllAsync([FromServices] IRepository<Users> repository, [FromServices] IDataFilter dataFilter)
{
     using (dataFilter.Disable<ISoftDelete>())
     {
         // Temporarily disable soft delete filtering and query data that has been marked for deletion
         var list = (await repository.GetListAsync()).ToList();
         return System.Text.Json.JsonSerializer.Serialize(list);
     }
}
```