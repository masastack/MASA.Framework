[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EntityFrameworkCore.Cosmos

## Example:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.Cosmos
```

##### Usage 1:

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "AccountKey=AccountKey;AccountEndpoint=AccountEndpoint;Database=Database"// or "ConnectionString=ConnectionString;Database=Database"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseCosmos());
```

##### Usage 2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseCosmos($"{accountEndpoint}",$"{accountKey}",$"{databaseName}"));
//builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseCosmos($"{connectionString}",$"{databaseName}"));
```