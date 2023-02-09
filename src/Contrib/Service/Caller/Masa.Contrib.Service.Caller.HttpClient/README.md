[中](README.zh-CN.md) | EN

## Masa.Contrib.Service.Caller.HttpClient

Provide service calls based on `HttpClient`, and the subsequent migration to `Dapr`-based service calls costs less

Example:

``` powershell
Install-Package Masa.Contrib.Service.Caller.HttpClient
```

### Get Started

1. Register `Caller`, modify `Program.cs`

``` C#
builder.Services.AddCaller("UserCaller", options =>
{
    options.UseHttpClient(client => client.BaseAddress = "http://localhost:5000");
});
```

2. How to use:

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] ICaller caller, string name)
    => caller.GetAsync<string>($"/Hello", new { Name = name }));
```

> The interface address of the complete request is: http://localhost:5000/Hello?Name={name}

3. When there are multiple HttpClients, modify `Program.cs`

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

4. How to use UserCaller or OrderCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] ICaller caller, string name)
    => caller.GetAsync<string>($"/Hello", new { Name = name }));// Get UserCaller

app.MapGet("/Test/Order/Hello", ([FromServices] ICallerFactory callerFactory, string name) =>
{
    var caller = callerFactory.Create("OrderCaller");
    return caller.GetAsync<string>($"/Hello", new { Name = name });
});
```

> When multiple Callers are added, how to get the specified Caller?
>> Get the Caller of the specified alias through the `Create` method of `CallerFactory`
>
> Why doesn't `caller` get the corresponding Caller through the `Create` method of `CallerFactory`?
>> If no default ICaller is specified, the default Caller is the first one added in the `AddCaller` method

### Recommend

1. Register `Caller`, automatically register `Caller` according to the convention, modify `Program.cs`

``` C#
builder.Services.AddCaller();
```

2. Add a new class `UserCaller`, and inherit `HttpClientCallerBase`, configure the domain address

``` C#
public class UserCaller: HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:5000";

    public Task<string> HelloAsync(string name) => Caller.GetStringAsync($"/Hello", new { Name = name });

    /// <summary>
    /// There is no need to overload by default, and it can be overloaded when there are special requirements for httpClient
    /// </summary>
    /// <param name="httpClient"></param>
    protected override void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
        httpClient.Timeout = TimeSpan.FromSeconds(5);
    }
}
```

3. How to use UserCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
    => caller.HelloAsync(name));
```