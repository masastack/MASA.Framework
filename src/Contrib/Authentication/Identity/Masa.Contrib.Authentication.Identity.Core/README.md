[中](README.zh-CN.md) | EN

## Authentication.Identity.Core

Identity core library, providing core implementation for `Identity`, it needs to be used with the following three libraries

* [Authentication.Identity](../Masa.Contrib.Authentication.Identity/README.zh-CN.md): Provide `Identity` service for Mvc, MinimalAPI projects
* [Authentication.Identity.BlazorServer](../Masa.Contrib.Authentication.Identity.BlazorServer/README.zh-CN.md): Provide `Identity` service for BlazorServer project
* [Authentication.Identity.BlazorAssembly](../Masa.Contrib.Authentication.Identity.BlazorAssembly/README.zh-CN.md): Provide `Identity` service for BlazorAssembly project

Let's take `Authentication.Identity` as an example:

Example:

``` C#
Install-Package Masa.Contrib.Authentication.Identity
```

1. Modify `Program.cs`

``` C#
builder.Services.AddMasaIdentity();
```

2. Obtain user information

``` C#
IUserContext userContext;//Get IUserContext from DI
userContext.User;//Get user information
```

> The default user information is obtained from HttpContext.User
>
> UserId: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_USER_ID
>
> UserName: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_USER_NAME
>
> TenantId: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_TENANT_ID
>
> Environment: Masa.Contrib.IdentityModel.Const.ClaimType.DEFAULT_ENVIRONMENT

3. Temporarily change the current login user information

``` C#
IUserSetter userSetter;//Get IUserSetter from DI
var user = new IdentityUser()
{
    Id = "2",
    UserName = "Tom",
    Environment = "Production",
    TenantId = "2"
};
using(userSetter.Change(user))
{
    //The obtained user information is Tom
}
```