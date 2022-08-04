[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.Cosmos

## Example:

```c#
Install-Package Masa.Contrib.Data.EFCore.Cosmos
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
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseCosmos());
```

##### Usage 2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseCosmos($"{accountEndpoint}",$"{accountKey}",$"{databaseName}"));
//builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseCosmos($"{connectionString}",$"{databaseName}"));
```