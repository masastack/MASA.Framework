[中](README.zh-CN.md) | EN

## Masa.Contrib.Identity

Example:

```` C#
Install-Package Masa.Contrib.Identity.IdentityModel
````

1. Modify `Program.cs`

```` C#
builder.Services.AddMasaIdentityModel();
````

2. Get user information

```` C#
IUserContext userContext;//Get IUserContext from DI
userContext.User;//Get user information
````

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

```` C#
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
````