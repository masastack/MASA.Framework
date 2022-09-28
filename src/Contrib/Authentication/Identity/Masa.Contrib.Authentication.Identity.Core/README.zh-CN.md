中 | [EN](README.md)

## Authentication.Identity.Core

Identity核心库，为`Identity`提供核心实现，它需要配合以下三个库来使用

* [Authentication.Identity](../Masa.Contrib.Authentication.Identity/README.zh-CN.md): 为Mvc、Minimal APIs项目提供`Identity`服务
* [Authentication.Identity.BlazorServer](../Masa.Contrib.Authentication.Identity.BlazorServer/README.zh-CN.md): 为BlazorServer项目提供`Identity`服务
* [Authentication.Identity.BlazorAssembly](../Masa.Contrib.Authentication.Identity.BlazorAssembly/README.zh-CN.md): 为BlazorAssembly项目提供`Identity`服务

下面我们以`Authentication.Identity`为例：

用例：

``` C#
Install-Package Masa.Contrib.Authentication.Identity
```

1. 修改`Program.cs`

``` C#
builder.Services.AddMasaIdentity();
```

2. 获取用户信息

``` C#
IUserContext userContext;//从DI中获取IUserContext
userContext.User;//获取用户信息
```

> 默认用户信息从HttpContext.User中获取
>
> UserId: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_USER_ID
>
> UserName: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_USER_NAME
>
> TenantId: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_TENANT_ID
>
> Environment: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_ENVIRONMENT

3. 临时更改当前登录用户信息

``` C#
IUserSetter userSetter;//从DI中获取IUserSetter
var user = new IdentityUser()
{
    Id = "2",
    UserName = "Tom",
    Environment = "Production",
    TenantId = "2"
};
using (userSetter.Change(user))
{
    //获取到的用户信息为Tom
}
```
