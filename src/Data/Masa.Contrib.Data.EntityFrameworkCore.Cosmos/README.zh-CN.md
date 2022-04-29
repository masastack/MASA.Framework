中 | [EN](README.md)

## Masa.Contrib.Data.EntityFrameworkCore.Cosmos

## 用例:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.Cosmos
```

#### 用法1:

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "AccountKey=AccountKey;AccountEndpoint=AccountEndpoint;Database=Database"//或"ConnectionString=ConnectionString;Database=Database"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseCosmos());
```

#### 用法2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseCosmos($"{accountEndpoint}",$"{accountKey}",$"{databaseName}"));
//builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseCosmos($"{connectionString}",$"{databaseName}"));
```