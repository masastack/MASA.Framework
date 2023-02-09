中 | [EN](README.md)

## Masa.Contrib.Service.Caller.HttpClient

提供基于`HttpClient`的服务调用，后续迁移到基于`Dapr`的服务调用成本更低

用例:

``` powershell
Install-Package Masa.Contrib.Service.Caller.HttpClient
```

### 入门

1. 注册`Caller`，并手动注册`HttpCaller`，修改`Program.cs`

``` C#
builder.Services.AddCaller("UserCaller", options =>
{
    options.UseHttpClient(client => client.BaseAddress = "http://localhost:5000");
});
```

2. 如何使用:

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] ICaller caller, string name)
    => caller.GetAsync<string>($"/Hello", new { Name = name }));
```

> 完整请求的接口地址是：http://localhost:5000/Hello?Name={name}

3. 当存在多个HttpClient时，则修改`Program.cs`为

``` C#
builder.Services.AddCaller("UserCaller", options =>
{
    options.UseHttpClient(client => client.BaseAddress = "http://localhost:5000");
});
builder.Services.AddCaller("OrderCaller", options =>
{
    options.UseHttpClient("OrderCaller", client => client.BaseAddress = "http://localhost:6000");
});
```

4. 如何使用UserCaller或OrderCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] ICaller caller, string name)
    => caller.GetAsync<string>($"/Hello", new { Name = name })); // 获取到的是UserCaller

app.MapGet("/Test/Order/Hello", ([FromServices] ICallerFactory callerFactory, string name) =>
{
    var orderCaller = callerFactory.Create("OrderCaller");
    return orderCaller.GetAsync<string>($"/Hello", new { Name = name });
});
```

> 当多个Caller被添加时，如何获取指定的Caller？
>> 通过`CallerFactory`的`Create`方法得到指定别名的Caller
>
> 为什么`caller`没有通过`CallerFactory`的`Create`方法得到对应的Caller？
>> 如果未指定默认的ICallerProvider，则在`AddCaller`方法中第一个被添加的就是默认的Caller

### 推荐

1. 注册`Caller`，根据约定自动注册`Caller`，修改`Program.cs`

``` C#
builder.Services.AddCaller();
```

2. 新增加类`UserCaller`，并继承`HttpClientCallerBase`，配置域地址

``` C#
public class UserCaller: HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:5000";

    public Task<string> HelloAsync(string name) => Caller.GetStringAsync($"/Hello", new { Name = name });

    /// <summary>
    /// 默认不需要重载，对httpClient有特殊需求时可重载
    /// </summary>
    /// <param name="httpClient"></param>
    protected override void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
        httpClient.Timeout = TimeSpan.FromSeconds(5);
    }
}
```

3. 如何使用UserCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
    => caller.HelloAsync(name));
```