[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.InMemory

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.InMemory
Install-Package Masa.Contrib.Data.Contracts.EFCore //Use the data filtering and soft delete capabilities provided by the protocol, if you don't need it, you can not refer to it
```

### Usage 1

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "identity"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts.EFCore
    optionsBuilder.UseInMemoryDatabase(); //Use an in-memory database
});
```

### Usage 2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts.EFCore
    optionsBuilder.UseInMemoryDatabase("identity"); //Use an in-memory database
});
```