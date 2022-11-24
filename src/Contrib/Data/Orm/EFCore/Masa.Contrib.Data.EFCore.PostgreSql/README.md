[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore.PostgreSql

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.PostgreSql
Install-Package Masa.Contrib.Data.Contracts //Use the data filtering and soft delete capabilities provided by the protocol, if you don't need it, you can not refer to it
```

### Usage 1

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"
  }
}
```

2. Register `MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts
    optionsBuilder.UseNpgsql(); //Use PostgreSQL database
});
```

### Usage 2

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter(); //Enable filtering, provided by Masa.Contrib.Data.Contracts
    optionsBuilder.UseNpgsql("Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"); //Use PostgreSQL database
});
```

> For the link string, please refer to: https://www.connectionstrings.com/postgresql