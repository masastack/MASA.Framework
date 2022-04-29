中 | [EN](README.md)

## Masa.Contrib.Data.EntityFrameworkCore.PostgreSql

## 用例:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.PostgreSql
```

#### 用法1:

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseNpgsql());
```

#### 用法2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseNpgsql("Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity"));
```