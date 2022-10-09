[中](README.zh-CN.md) | EN

## Masa.Contrib.Service.Caller.Authentication.OpenIdConnect

`Masa.Contrib.Service.Caller.Authentication.OpenIdConnect` is an extension library for `Caller` about authentication, which enables your project to carry credential information when sending requests to complete authentication

Example:

``` c#
Install-Package Masa.Contrib.Service.Caller.HttpClient //Caller implementation, choose to use HttpClient or DaprClient according to the actual situation
Install-Package Masa.Contrib.Service.Caller.Authentication.OpenIdConnect //Requires authentication and authorization
```

### Get Started

1. Register `Caller`, use automatic registration of Caller and authentication, modify `Program.cs`

``` C#
builder.Services.AddCaller(options =>
{
     options.UseAuthentication();
});
```

2. Add a new class `UserCaller`, and inherit `HttpClientCallerBase` to configure the domain address

``` C#
public class UserCaller: HttpClientCallerBase
{
     protected override string BaseAddress { get; set; } = "http://localhost:5000";

     public Task<string> HelloAsync(string name) => Caller.GetStringAsync($"/Hello", new { Name = name });
}
```

3. How to use UserCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
     => caller.HelloAsync(name));
```