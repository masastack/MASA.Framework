中 | [EN](README.md)

## Masa.Contrib.Data.EntityFrameworkCore.MySql

## 用例:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.MySql
```

#### 用法1:

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=identity;Uid=myUsername;Pwd=P@ssw0rd;"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseMySQL());
```

#### 用法2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseMySQL("Server=localhost;Database=identity;Uid=myUsername;Pwd=P@ssw0rd;"));
```