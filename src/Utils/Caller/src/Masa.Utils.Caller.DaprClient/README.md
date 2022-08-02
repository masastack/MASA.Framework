[中](README.zh-CN.md) | EN

## Masa.Utils.Caller.DaprClient

## Example:

````c#
Install-Package Masa.Utils.Caller.DaprClient
````

### Basic usage:

1. Modify `Program.cs`

    ```` C#
    builder.Services.AddCaller(options =>
    {
        options.UseDapr(clientBuilder =>
        {
            clientBuilder.Name = "UserCaller";// The alias of the current Caller, when there is only one Provider, you can not assign a value to Name
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//AppID of the callee dapr
        });
    });
    ````

2. How to use:

    ```` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICallerProvider userCallerProvider, string name)
        => userCallerProvider.GetAsync<string>($"/Hello", new { Name = name }));
    ````

   > The interface address of the complete request is: http://localhost:3500/v1.0/invoke/<Replace-You-Dapr-AppID>/method/Hello?Name={name}

3. When there are multiple DaprClients, modify `Program.cs`

    ```` C#
    builder.Services.AddCaller(options =>
    {
        options.UseDapr(clientBuilder =>
        {
            clientBuilder.Name = "UserCaller";
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//AppID of the callee User service Dapr
        });
        options.UseDapr(clientBuilder =>
        {
            clientBuilder.Name = "OrderCaller";
            clientBuilder.AppId = "<Replace-You-Dapr-AppID>" ;//AppID of the callee Order service Dapr
        });
    });
    ````

4. How to use UserCaller or OrderCaller

    ```` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICallerProvider userCallerProvider, string name)
        => userCallerProvider.GetAsync<string>($"/Hello", new { Name = name }));


    app.MapGet("/Test/Order/Hello", ([FromServices] ICallerFactory callerFactory, string name) =>
    {
        var callerProvider = callerFactory.CreateClient("OrderCaller");
        return callerProvider.GetAsync<string>($"/Hello", new { Name = name });
    });
    ````

> When multiple Callers are added, how to get the specified Caller?
>> Get the CallerProvider of the specified alias through the `CreateClient` method of `CallerFactory`
>
> Why doesn't `userCallerProvider` get the corresponding Caller through the `CreateClient` method of `CallerFactory`?
>> If no default ICallerProvider is specified, the default CallerProvider is the first one added in the `AddCaller` method

### Recommended usage

1. Modify `Program.cs`

    ```` C#
    builder.Services.AddCaller();
    ````

2. Add a new class `UserCaller`

    ```` C#
    public class UserCaller: DaprCallerBase
    {
        protected override string AppId { get; set; } = "<Replace-You-UserService-Dapr-AppID>";

        public HttpCaller(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<string> HelloAsync(string name) => CallerProvider.GetStringAsync($"/Hello", new { Name = name });
    }
    ````

3. How to use UserCaller

    ```` C#
    app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
        => caller.HelloAsync(name));
    ````