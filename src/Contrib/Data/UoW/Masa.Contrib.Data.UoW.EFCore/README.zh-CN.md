中 | [EN](README.md)

## Masa.Contrib.Data.UoW.EFCore

基于`EFCore`实现的工作单元

用例：

``` powershell
Install-Package Masa.Contrib.Dispatcher.Events
Install-Package Masa.Contrib.Data.UoW.EFCore
Install-Package Masa.Contrib.Data.EFCore.SqlServer
```

### 入门

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
