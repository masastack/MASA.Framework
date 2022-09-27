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
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"
  }
}
```

2. Use UoW

```C#
builder.Services.AddEventBus(eventBusBuilder => eventBusBuilder.UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlServer()));
```