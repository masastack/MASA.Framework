中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.Cosmos

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.Cosmos
Install-Package Masa.Contrib.Data.Contracts //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 用法1

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "AccountKey=<账号密钥>;AccountEndpoint=<账号终结点>;Database=<数据库名称>" //或"ConnectionString=<连接字符串>;Database=<数据库名称>"
  }
}
```

2. 注册MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts 提供
    optionsBuilder.UseCosmos(); //使用Cosmos数据库
});
```

### 用法2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts 提供
    optionsBuilder.UseCosmos($"{accountEndpoint}",$"{accountKey}",$"{databaseName}"); //使用Cosmos数据库
});
```