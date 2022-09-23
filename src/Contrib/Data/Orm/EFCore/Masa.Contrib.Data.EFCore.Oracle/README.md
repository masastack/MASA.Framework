[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.Oracle

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.Oracle
Install-Package Masa.Contrib.Data.Contracts.EFCore //Use the data filtering and soft delete capabilities provided by the protocol, if you don't need it, you can not refer to it
```

### Usage 1

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=MyOracleDB;Integrated Security=yes;"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts.EFCore
    optionsBuilder.UseOracle(); //Use Oracle database
});
```

### Usage 2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts.EFCore
    optionsBuilder.UseOracle("Data Source=MyOracleDB;Integrated Security=yes;"); //Use Oracle database
});
```

> For the link string, please refer to: https://www.connectionstrings.com/oracle