中 | [EN](README.md)

## Masa.Contrib.Data.EntityFrameworkCore.SqlServer

## 用例:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore.SqlServer
```

#### 用法1:

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseSqlServer());
```

#### 用法2:

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder => optionsBuilder.UseSoftDelete().UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"));
```