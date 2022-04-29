[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EntityFrameworkCore.Sqlite

## Example:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.Sqlite
```

##### Usage 1:

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=c:\mydb.db;Version=3;Password=P@ssw0rd;"
  }
}
```

2. Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseSqlite());
```

##### Usage 2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseSqlite("Data Source=c:\mydb.db;Version=3;Password=P@ssw0rd;"));
```