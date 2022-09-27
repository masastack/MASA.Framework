[中](README.zh-CN.md) | EN

## Authentication.Identity.BlazorAssembly

Provides `Identity` implementation, supports `Blazor Assembly` projects

Example:

``` C#
Install-Package Masa.Contrib.Authentication.Identity.BlazorAssembly
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
     //The obtained user information is Tom
}
//The user information obtained again is the original logged in user
```