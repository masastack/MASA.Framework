中 | [EN](README.md)

## MinimalAPI

原始用法：

```C#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/api/v1/helloworld", ()=>"Hello World");
app.Run();
```

用例：

```c#
Install-Package Masa.Contrib.Service.MinimalAPIs
```

1. 添加MinimalAPI

```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Services
                 .AddServices(builder);
```

2. 自定义Service并继承ServiceBase，如：

```c#
public class IntegrationEventService : ServiceBase
{
    public IntegrationEventService(IServiceCollection services) : base(services)
    {
        App.MapGet("/api/v1/payment/HelloWorld", HelloWorld);
    }

    public string HelloWorld()
    {
        return "Hello World";
    }
}
```

> 提示：继承ServiceBase的服务为单例模式注册，如果需要从DI获取获取

```C#
public async Task DeleteBasketByIdAsync(string id, [FromServices] IBasketRepository repository)
{
    await repository.DeleteBasketAsync(id);
}
```

