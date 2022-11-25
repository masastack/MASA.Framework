[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.Cosmos

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.Cosmos
Install-Package Masa.Contrib.Data.Contracts //Use the data filtering and soft delete capabilities provided by the protocol, if you don't need it, you can not refer to it
```

### Usage 1

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "AccountKey=AccountKey;AccountEndpoint=AccountEndpoint;Database=Database" //or "ConnectionString=ConnectionString;Database=Database"
  }
}
```

2. Register `MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts
    optionsBuilder.UseCosmos(); //Use Cosmos database
});
```

### Usage 2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts
    optionsBuilder.UseCosmos($"{accountEndpoint}",$"{accountKey}",$"{databaseName}"); //Use Cosmos database
});
```