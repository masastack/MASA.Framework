中 | [EN](README.md)

## Masa.Contrib.Identity

用例：

``` C#
Install-Package Masa.Contrib.Authentication.Identity
```

1. 修改`Program.cs`

``` C#
builder.Services.AddMasaIdentityModel();
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
