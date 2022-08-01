[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EntityFrameworkCore.Oracle

## Example:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.Oracle
```

##### Usage 1:

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
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseOracle());
```

##### Usage 2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseOracle("Data Source=MyOracleDB;Integrated Security=yes;"));
```