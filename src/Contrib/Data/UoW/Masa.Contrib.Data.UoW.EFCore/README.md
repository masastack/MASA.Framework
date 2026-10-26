[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.UoW.EFCore

Unit of work based on `EFCore` implementation

Example：

``` powershell
Install-Package Masa.Contrib.Dispatcher.Events
Install-Package Masa.Contrib.Data.UoW.EFCore
Install-Package Masa.Contrib.Data.EFCore.SqlServer
```

### Get Started

1. Configure appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=<your server>;uid=<database account>;pwd=<database password>;database=<your database>"
  }
}
```

2. Use UoW

```C#
builder.Services.AddEventBus(eventBusBuilder => eventBusBuilder.UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlServer()));
```