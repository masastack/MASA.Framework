[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EntityFrameworkCore.PostgreSql

## Example:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.PostgreSql
```

##### Usage 1:

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseNpgsql());
```

##### Usage 2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseNpgsql("Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"));
```