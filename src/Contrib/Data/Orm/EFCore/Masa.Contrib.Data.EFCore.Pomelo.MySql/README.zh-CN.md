中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.Pomelo.MySql

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.Pomelo.MySql
Install-Package Masa.Contrib.Data.Contracts //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 用法1

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<服务器地址>;port=<端口>;Database=<数据库名称>;Uid=<数据库账号>;Pwd=<数据库密码>;"
  }
}
```

2. 注册`MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts 提供
    optionsBuilder.UseMySql(new MySqlServerVersion("5.7.26")); //使用MySql数据库
});
```

### 用法2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //启用过滤，由 Masa.Contrib.Data.Contracts 提供
    optionsBuilder.UseMySql("Server=<服务器地址>;port=<端口>;Database=<数据库名称>;Uid=<数据库账号>;Pwd=<数据库密码>;", new MySqlServerVersion("5.7.26")); //使用MySql数据库
});
```

> 链接字符串可参考：https://www.connectionstrings.com/mysql