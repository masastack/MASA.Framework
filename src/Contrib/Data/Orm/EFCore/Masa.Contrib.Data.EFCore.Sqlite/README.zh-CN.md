中 | [EN](README.md)

## Masa.Contrib.Data.EFCore.Sqlite

## 用例:

```c#
Install-Package Masa.Contrib.Data.EFCore.Sqlite
```

#### 用法1:

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=c:\mydb.db;Version=3;Password=P@ssw0rd;"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseSqlite());
```

#### 用法2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseFilter().UseSqlite("Data Source=c:\mydb.db;Version=3;Password=P@ssw0rd;"));
```