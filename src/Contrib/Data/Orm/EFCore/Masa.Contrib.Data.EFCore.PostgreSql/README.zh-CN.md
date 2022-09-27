中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.PostgreSql

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.PostgreSql
Install-Package Masa.Contrib.Data.Contracts.EFCore //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 用法1

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"
  }
}
```

2. 注册`MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseNpgsql(); //使用PostgreSQL数据库
});
```

### 用法2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseNpgsql("Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"); //使用PostgreSQL数据库
});
```

> 链接字符串可参考：https://www.connectionstrings.com/postgresql