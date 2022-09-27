中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.SqlServer

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.SqlServer
Install-Package Masa.Contrib.Data.Contracts.EFCore //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 用法1

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"
  }
}
```

2. 注册`MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseSqlServer(); //使用SqlServer数据库
});
```

### 用法2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"); //使用SqlServer数据库
});
```

> 链接字符串可参考：https://www.connectionstrings.com/sql-server