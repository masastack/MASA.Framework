中 | [EN](README.md)

## Masa.Contrib.Service.Caller.Authentication.OpenIdConnect

`Masa.Contrib.Service.Caller.Authentication.OpenIdConnect`是`Caller`关于认证的扩展库，它可以使得你的项目在发送请求时会携带凭证信息，用来完成认证

用例:

``` powershell
Install-Package Masa.Contrib.Service.Caller.HttpClient //Caller的实现，根据实际情况选择使用HttpClient或者DaprClient
Install-Package Masa.Contrib.Service.Caller.Authentication.OpenIdConnect //需要认证授权
```

### 入门

1. 注册`Caller`，使用自动注册Caller与认证，修改`Program.cs`

``` C#
builder.Services.AddCaller(options =>
{
    options.UseAuthentication();
});
```

2. 定义中间件为认证信息赋值 (非Blazor项目)

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
        tokenProvider.AccessToken = "{Replace-Your-AccessToken}";//访问凭证赋值
        tokenProvider.RefreshToken = "{Replace-Your-RefreshToken}";//刷新凭证赋值
        tokenProvider.IdToken = "{Replace-Your-IdToken}";//身份凭证赋值
        await _next.Invoke(httpContext);
    }
}
```

> Blazor Server项目不建议使用中间件赋值，可通过在`_Host.cshtml`中通过HttpContext获取当前凭证并赋值

3. 使用中间件，修改`Program.cs`

``` C#
//在映射路由之前，确保进入方法前中间件已经执行
app.UseMiddleware<TokenProviderMiddleware>();
```

4. 新增加类`UserCaller`，并继承`HttpClientCallerBase`，配置域地址

``` C#
public class UserCaller: HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:5000";

    public Task<string> HelloAsync(string name) => Caller.GetStringAsync($"/Hello", new { Name = name });
}
```

4. 如何使用UserCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
    => caller.HelloAsync(name));
```