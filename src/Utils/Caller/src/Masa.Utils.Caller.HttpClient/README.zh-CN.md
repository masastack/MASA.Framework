中 | [EN](README.md)

## Masa.Utils.Caller.HttpClient

## 用例:

```c#
Install-Package Masa.Utils.Caller.HttpClient
```

### 基本用法:

1. 修改`Program.cs`

    ``` C#
    builder.Services.AddCaller(options =>
    {
        options.UseHttpClient(httpClientBuilder =>
        {
            httpClientBuilder.Name = "UserCaller";// 当前Caller的别名，仅存在一个HttpClient时，可以不对Name赋值
            httpClientBuilder.BaseAddress = "http://localhost:5000" ;
        });
    });
    ```

2. 如何使用:

    ``` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICallerProvider userCallerProvider, string name)
        => userCallerProvider.GetAsync<string>($"/Hello", new { Name = name }));
    ```

   > 完整请求的接口地址是：http://localhost:5000/Hello?Name={name}

3. 当存在多个HttpClient时，则修改`Program.cs`为

    ``` C#
    builder.Services.AddCaller(options =>
    {
        options.UseHttpClient(httpClientBuilder =>
        {
            httpClientBuilder.Name = "UserCaller";
            httpClientBuilder.BaseAddress = "http://localhost:5000" ;
        });
        options.UseHttpClient(httpClientBuilder =>
        {
            httpClientBuilder.Name = "OrderCaller";
            httpClientBuilder.BaseAddress = "http://localhost:6000" ;
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
    public class UserCaller: HttpClientCallerBase
    {
        protected override string BaseAddress { get; set; } = "http://localhost:5000";

        public HttpCaller(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<string> HelloAsync(string name) => CallerProvider.GetStringAsync($"/Hello", new { Name = name });

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