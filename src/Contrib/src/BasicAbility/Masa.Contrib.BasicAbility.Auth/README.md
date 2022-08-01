[中](README.zh-CN.md) | EN

## Masa.Contrib.BasicAbility.Auth

Injecting IAuthClient interface，cal the service to obtain the capabilities provided by the auth SDK.
SDK need to get current user ID dependency Masa.Contrib.Identity.IdentityModel(../../Identity/Masa.Contrib.Identity.IdentityModel/README.zh，Therefore,the identitymodel service needs to be added before use.

### Service Introduction

```c#
IAuthClient
├── UserService                     
├── SubjectService                  
├── TeamService                     
├── PermissionService               
└── ProjectService                  
```

### Use Introduction

#### Install dependent package

```C#
Install-Package Masa.Contrib.BasicAbility.Auth
```

#### Register auth service

```C#
builder.Services.AddAuthClient("http://authservice.com");
```

> `http://authservice.com` is auth service address

#### Dependency injection IAuthClient

```c#
var app = builder.Build();

app.MapGet("/GetTeams", ([FromServices] IAuthClient authClient) =>
{
    return authClient.TeamService.GetAllAsync();
});

app.Run();
```
