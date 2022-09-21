中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.Cosmos

## 用例:

```c#
Install-Package Masa.Contrib.Data.EFCore.Cosmos
Install-Package Masa.Contrib.Data.Contracts.EFCore //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 用法1:

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "AccountKey=AccountKey;AccountEndpoint=AccountEndpoint;Database=Database" //或"ConnectionString=ConnectionString;Database=Database"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseCosmos(); //使用Cosmos数据库
});
```

### 用法2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseCosmos($"{accountEndpoint}",$"{accountKey}",$"{databaseName}"); //使用Cosmos数据库
});
```