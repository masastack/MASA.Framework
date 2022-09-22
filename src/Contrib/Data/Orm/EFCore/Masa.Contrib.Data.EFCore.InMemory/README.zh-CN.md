中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.InMemory

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.InMemory
Install-Package Masa.Contrib.Data.Contracts.EFCore //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 用法1

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "identity"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseInMemoryDatabase(); //使用内存数据库，也可自行选择其它实现
});
```

### 用法2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseInMemoryDatabase("identity"); //使用内存数据库，也可自行选择其它实现
});
```