中 | [EN](README.md)

## Masa.Utils.Caller.DaprClient

## 用例:

```c#
Install-Package Masa.Utils.Caller.DaprClient
```

### 基本用法:

1. 修改`Program.cs`

    ``` C#
    builder.Services.AddCaller(options =>
    {
        options.UseDapr(clientBuilder =>
        {
            clientBuilder.Name = "UserCaller";// 当前Caller的别名，仅存在一个Provider时，可以不对Name赋值
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//被调用方dapr的AppID
        });
    });
    ```

2. 如何使用:

    ``` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICallerProvider userCallerProvider, string name)
        => userCallerProvider.GetAsync<string>($"/Hello", new { Name = name }));
    ```

    > 完整请求的接口地址是：http://localhost:3500/v1.0/invoke/<Replace-You-Dapr-AppID>/method/Hello?Name={name}

3. 当存在多个DaprClient时，则修改`Program.cs`为

    ``` C#
    builder.Services.AddCaller(options =>
    {
        options.UseDapr(clientBuilder =>
        {
            clientBuilder.Name = "UserCaller";
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//被调用方User服务Dapr的AppID
        });
        options.UseDapr(clientBuilder =>
        {
            clientBuilder.Name = "OrderCaller";
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//被调用方Order服务Dapr的AppID
        });
    });
    ```

4. 如何使用UserCaller或OrderCaller

    ``` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICallerProvider userCallerProvider, string name)
        => userCallerProvider.GetAsync<string>($"/Hello", new { Name = name }));


    app.MapGet("/Test/Order/Hello", ([FromServices] ICallerFactory callerFactory, string name) =>
    {
        var callerProvider = callerFactory.CreateClient("OrderCaller");
        return callerProvider.GetAsync<string>($"/Hello", new { Name = name });
    });
    ```

> 当多个Caller被添加时，如何获取指定的Caller？
>> 通过`CallerFactory`的`CreateClient`方法得到指定别名的CallerProvider
>
> 为什么`userCallerProvider`没有通过`CallerFactory`的`CreateClient`方法得到对应的Caller？
>> 如果未指定默认的ICallerProvider，则在`AddCaller`方法中第一个被添加的就是默认的CallerProvider

### 推荐用法

1. 修改`Program.cs`

    ``` C#
    builder.Services.AddCaller();
    ```

2. 新增加类`UserCaller`

    ``` C#
    public class UserCaller: DaprCallerBase
    {
        protected override string AppId { get; set; } = "<Replace-You-UserService-Dapr-AppID>";

        public HttpCaller(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<string> HelloAsync(string name) => CallerProvider.GetStringAsync($"/Hello", new { Name = name });
    }
    ```

3. 如何使用UserCaller

    ``` C#
    app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
        => caller.HelloAsync(name));
    ```