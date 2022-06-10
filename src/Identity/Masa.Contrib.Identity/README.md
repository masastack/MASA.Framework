[中](README.zh-CN.md) | EN

## Masa.Contrib.Identity

Example:

```` C#
Install-Package Masa.Contrib.Identity
````

1. Modify `Program.cs`

```` C#
builder.Services.AddMasaIdentity();
````

2. Obtain user information

```` C#
IUserContext userContext;//Get IUserContext from DI
userContext.User;//Get user information
````

> The default user information is obtained from HttpContext.User
>
> UserId: ClaimTypes.NameIdentifier
>
> UserName: ClaimTypes.Name
>
> TenantId: tenantid
>
> Environment: environment

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