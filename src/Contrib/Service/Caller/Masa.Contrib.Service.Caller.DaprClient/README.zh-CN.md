中 | [EN](README.md)

## Masa.Contrib.Service.Caller.DaprClient

用例:

```c#
Install-Package Masa.Contrib.Service.Caller
Install-Package Masa.Contrib.Service.Caller.DaprClient
```

### 基本用法:

1. 修改`Program.cs`

    ``` C#
    builder.Services.AddCaller(options =>
    {
        options.UseDapr("UserCaller", clientBuilder =>
        {
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//被调用方dapr的AppID
        });
    });
    ```

2. 如何使用:

    ``` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICaller userCaller, string name)
        => userCaller.GetAsync<string>($"/Hello", new { Name = name }));
    ```

    > 完整请求的接口地址是：http://localhost:3500/v1.0/invoke/<Replace-You-Dapr-AppID>/method/Hello?Name={name}

3. 当存在多个DaprClient时，则修改`Program.cs`为

    ``` C#
    builder.Services.AddCaller(options =>
    {
        options.UseDapr("UserCaller", clientBuilder =>
        {
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//被调用方User服务Dapr的AppID
        });
        options.UseDapr("OrderCaller", clientBuilder =>
        {
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//被调用方Order服务Dapr的AppID
        });
    });
    ```

4. 如何使用UserCaller或OrderCaller

    ``` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICaller userCaller, string name)
        => userCaller.GetAsync<string>($"/Hello", new { Name = name }));

    app.MapGet("/Test/Order/Hello", ([FromServices] ICallerFactory callerFactory, string name) =>
    {
        var caller = callerFactory.CreateClient("OrderCaller");
        return caller.GetAsync<string>($"/Hello", new { Name = name });
    });
    ```

> 当多个Caller被添加时，如何获取指定的Caller？
>> 通过`CallerFactory`的`CreateClient`方法得到指定别名的Caller
>
> 为什么`userCaller`没有通过`CallerFactory`的`CreateClient`方法得到对应的Caller？
>> 如果未指定默认的ICaller，则在`AddCaller`方法中第一个被添加的就是默认的Caller

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

        public Task<string> HelloAsync(string name) => Caller.GetStringAsync($"/Hello", new { Name = name });
    }
    ```

3. 如何使用UserCaller

    ``` C#
    app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
        => caller.HelloAsync(name));
    ```