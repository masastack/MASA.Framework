[中](README.zh-CN.md) | EN

## MinimalAPI

Original usage：

```C#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/api/v1/IntegrationEvent/HelloWorld", () => "Hello World");
app.Run();
```

Example：

```c#
Install-Package Masa.Contrib.Service.MinimalAPIs
```

1. Add MinimalAPI

```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Services.AddServices(builder);
```

2. Custom Service and inherit ServiceBase

```c#
public class IntegrationEventService : ServiceBase
{
    public string HelloWorld()
    {
        return "Hello World";
    }
}
```

> Tip: The service that inherits ServiceBase is registered in singleton mode, if you need to obtain it from DI

```C#
public async Task DeleteBasketByIdAsync(string id, [FromServices] IBasketRepository repository)
{
    await repository.DeleteBasketAsync(id);
}
```

