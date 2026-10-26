[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.Pomelo.MySql

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.Pomelo.MySql
Install-Package Masa.Contrib.Data.Contracts //Use the data filtering and soft delete capabilities provided by the protocol, if you don't need it, you can not refer to it
```

### Usage 1

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<your server>;port=<port>;Database=<your database>;Uid=<database account>;Pwd=<database password>;"
  }
}
```

2. Register `MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts
    optionsBuilder.UseMySql(new MySqlServerVersion("5.7.26")); //Use MySql database
});
```

### Usage 2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts
    optionsBuilder.UseMySql("Server=<your server>;port=<port>;Database=<your database>;Uid=<database account>;Pwd=<database password>;", new MySqlServerVersion("5.7.26")); //Use MySql database
});
```

> For the link string, please refer to: https://www.connectionstrings.com/mysql