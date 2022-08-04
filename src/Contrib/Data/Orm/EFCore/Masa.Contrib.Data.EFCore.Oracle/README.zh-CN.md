中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.Oracle

## 用例:

```c#
Install-Package Masa.Contrib.Data.EFCore.Oracle
```

#### 用法1:

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=MyOracleDB;Integrated Security=yes;"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseOracle());
```

#### 用法2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseOracle("Data Source=MyOracleDB;Integrated Security=yes;"));
```