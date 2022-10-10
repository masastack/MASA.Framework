[中](README.zh-CN.md) | EN

## Masa.Contrib.Service.Caller.Authentication.OpenIdConnect

`Masa.Contrib.Service.Caller.Authentication.OpenIdConnect` is an extension library for `Caller` about authentication, which enables your project to carry credential information when sending requests to complete authentication

Example:

``` c#
Install-Package Masa.Contrib.Service.Caller.HttpClient //Caller implementation, choose to use HttpClient or DaprClient according to the actual situation
Install-Package Masa.Contrib.Service.Caller.Authentication.OpenIdConnect //Authentication and authorization required
```

### Get Started

1. Register `Caller`, use automatic registration of Caller and authentication, modify `Program.cs`

``` C#
builder.Services.AddCaller(options =>
{
     options.UseAuthentication();
});
```

2. Define middleware to assign value to authentication information (non-Blazor project)

``` C#
public class TokenProviderMiddleware
{
     private readonly RequestDelegate _next;

     public TokenProviderMiddleware(RequestDelegate next)
     {
         _next = next;
     }

     public async Task InvokeAsync(HttpContext httpContext)
     {
         var tokenProvider = httpContext.RequestServices.GetRequiredService<TokenProvider>();
         tokenProvider.AccessToken = "{Replace-Your-AccessToken}";//Access credential assignment
         tokenProvider.RefreshToken = "{Replace-Your-RefreshToken}";//Refresh credential assignment
         tokenProvider.IdToken = "{Replace-Your-IdToken}";//Identity Credential Assignment
         await _next.Invoke(httpContext);
     }
}
```

> The Blazor Server project does not recommend using middleware assignment. You can obtain the current credentials and assign them through HttpContext in `_Host.cshtml`

3. Using middleware, modify `Program.cs`

``` C#
//Before mapping routes, make sure the middleware has been executed before entering the method
app.UseMiddleware<TokenProviderMiddleware>();
```

4. Add a new class `UserCaller`, and inherit `HttpClientCallerBase` to configure the domain address

``` C#
public class UserCaller: HttpClientCallerBase
{
     protected override string BaseAddress { get; set; } = "http://localhost:5000";

     public Task<string> HelloAsync(string name) => Caller.GetStringAsync($"/Hello", new { Name = name });
}
```

5. How to use UserCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
     => caller.HelloAsync(name));
```