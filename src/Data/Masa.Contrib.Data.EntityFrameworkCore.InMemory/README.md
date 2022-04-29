[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EntityFrameworkCore.InMemory

## Example:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.InMemory
```

##### Usage 1:

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
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseInMemoryDatabase());
```

##### Usage 2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseInMemoryDatabase("identity"));
```