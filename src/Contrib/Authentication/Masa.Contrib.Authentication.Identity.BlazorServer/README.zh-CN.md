中 | [EN](README.md)

## Authentication.Identity.BlazorServer

为项目提供了`Identity`实现，支持`Blazor Server`项目

用例：

``` C#
Install-Package Masa.Contrib.Authentication.Identity.BlazorServer
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
    //在using范围内中获取到的是变更后的用户
}
//在using范围之外获取到的用户信息为原始登录用户
```
