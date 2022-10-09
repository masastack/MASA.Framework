中 | [EN](README.md)

## Masa.Contrib.Service.Caller.Authentication.OpenIdConnect

`Masa.Contrib.Service.Caller.Authentication.OpenIdConnect`是`Caller`关于认证的扩展库，它可以使得你的项目在发送请求时会携带凭证信息，用来完成认证

用例:

``` powershell
Install-Package Masa.Contrib.Service.Caller.HttpClient //Caller的实现，根据实际情况选择使用HttpClient或者DaprClient
Install-Package Masa.Contrib.Service.Caller.Authentication.OpenIdConnect //需摇认证授权
```

### 入门

1. 注册`Caller`，使用自动注册Caller与认证，修改`Program.cs`

``` C#
builder.Services.AddCaller(options =>
{
    options.UseAuthentication();
});
```

2. 新增加类`UserCaller`，并继承`HttpClientCallerBase`，配置域地址

``` C#
public class UserCaller: HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:5000";

    public Task<string> HelloAsync(string name) => Caller.GetStringAsync($"/Hello", new { Name = name });
}
```

3. 如何使用UserCaller

``` C#
app.MapGet("/Test/User/Hello", ([FromServices] UserCaller caller, string name)
    => caller.HelloAsync(name));
```