[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EntityFrameworkCore.Pomelo.MySql

## Example:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.Pomelo.MySql
```

##### Usage 1:

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=identity;Uid=myUsername;Pwd=P@ssw0rd;"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseMySql(Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.28-mysql")));
```

##### Usage 2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseMySql("Server=localhost;Database=identity;Uid=myUsername;Pwd=P@ssw0rd;", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.28-mysql")));
```