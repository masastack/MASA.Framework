中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.MySql

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.MySql
Install-Package Masa.Contrib.Data.Contracts //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 用法1

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;port=3306;Database=identity;Uid=myUsername;Pwd=P@ssw0rd;"
  }
}
```

2. 注册`MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts 提供
    optionsBuilder.UseMySQL(); //使用MySQL数据库
});
```

### 用法2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts 提供
    optionsBuilder.UseMySQL("Server=localhost;port=3306;Database=identity;Uid=myUsername;Pwd=P@ssw0rd;"); //使用MySQL数据库
});
```

> 链接字符串可参考：https://www.connectionstrings.com/mysql