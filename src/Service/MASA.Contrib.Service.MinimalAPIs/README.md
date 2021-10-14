## MinimalAPI

Original usage：

```C#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/api/v1/helloworld", ()=>"Hello World");
app.Run();
```

Example：

```c#
Install-Package MASA.Contrib.Service.MinimalAPIs
```

1. Add MinimalAPI

```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Services
				 .AddServices(builder);
```

2. Customize Service and inherit ServiceBase

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

> Tip: The service that inherits ServiceBase is registered in singleton mode, if you need to obtain it from DI

```C#
public async Task DeleteBasketByIdAsync(string id, [FromServices] IBasketRepository repository)
{
    await repository.DeleteBasketAsync(id);
}
```

