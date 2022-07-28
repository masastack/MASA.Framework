[中](README.zh-CN.md) | EN

## Masa.Contrib.Service.Caller.HttpClient

## Example:

````c#
Install-Package Masa.Contrib.Service.Caller
Install-Package Masa.Contrib.Service.Caller.HttpClient
````

### Basic usage:

1. Modify `Program.cs`

    ```` C#
    builder.Services.AddCaller(options =>
    {
        options.UseHttpClient(httpClientBuilder =>
        {
            httpClientBuilder.Name = "UserCaller";// The alias of the current Caller, when there is only one HttpClient, you can not assign a value to Name
            httpClientBuilder.BaseAddress = "http://localhost:5000" ;
        });
    });
    ````

2. How to use:

    ```` C#
    app.MapGet("/Test/User/Hello", ([FromServices] ICallerProvider userCallerProvider, string name)
        => userCallerProvider.GetAsync<string>($"/Hello", new { Name = name }));
    ````

    > The interface address of the complete request is: http://localhost:5000/Hello?Name={name}

3. When there are multiple HttpClients, modify `Program.cs`

    ```` C#
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
    public class UserCaller: HttpClientCallerBase
    {
        protected override string BaseAddress { get; set; } = "http://localhost:5000";

        public HttpCaller(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<string> HelloAsync(string name) => CallerProvider.GetStringAsync($"/Hello", new { Name = name });

        /// <summary>
        /// There is no need to overload by default, and it can be overloaded when there are special requirements for httpClient
        /// </summary>
        /// <param name="httpClient"></param>
        protected override void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(5);
        }
    }
    ````

3. How to use UserCaller

    ```` C#
    app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
        => caller.HelloAsync(name));
    ````