[中](README.zh-CN.md) | EN

## Authentication.Identity.BlazorServer

Provides an `Identity` implementation for the project, supports the `Blazor Server` project

Example:

``` C#
Install-Package Masa.Contrib.Authentication.Identity.BlazorServer
```

1. Modify `Program.cs`

``` C#
builder.Services.AddMasaIdentity();
```

2. Get user information

``` C#
IUserContext userContext;//Get IUserContext from DI
userContext.User;//Get user information
```

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
     //What is obtained in the using scope is the changed user
}
//The user information obtained outside the using scope is the original logged in user
```