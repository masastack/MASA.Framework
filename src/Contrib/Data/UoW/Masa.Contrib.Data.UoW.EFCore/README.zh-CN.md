中 | [EN](README.md)

## UoW.EF

用例：

```C#
Install-Package Masa.Contrib.Dispatcher.Events
Install-Package Masa.Contrib.Data.UoW.EFCore
Install-Package Masa.Contrib.Data.EFCore.SqlServer
```

1. 配置appsettings.json
``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"
  }
}
```

2. 使用UoW
```C#
builder.Services.AddEventBus(eventBusBuilder => eventBusBuilder.UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlServer()));
```
