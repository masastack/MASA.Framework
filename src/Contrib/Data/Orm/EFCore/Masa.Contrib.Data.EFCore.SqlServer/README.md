[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.SqlServer

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.SqlServer
Install-Package Masa.Contrib.Data.Contracts.EFCore //Use the data filtering and soft delete capabilities provided by the protocol, if you don't need it, you can not refer to it
```

### Usage 1

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts.EFCore
    optionsBuilder.UseSqlServer(); //Use SqlServer database
});
```

### Usage 2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts.EFCore
    optionsBuilder.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"); //Use SqlServer database
});
```

> For the link string, please refer to: https://www.connectionstrings.com/sql-server