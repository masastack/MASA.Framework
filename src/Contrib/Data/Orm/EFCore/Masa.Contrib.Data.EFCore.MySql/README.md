[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.MySql

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.MySql
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
    optionsBuilder.UseMySQL(); //Use MySQL database
});
```

### Usage 2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts
    optionsBuilder.UseMySQL("Server=<your server>;port=<port>;Database=<your database>;Uid=<database account>;Pwd=<database password>;"); //Use MySQL database
});
```

> For the link string, please refer to: https://www.connectionstrings.com/mysql