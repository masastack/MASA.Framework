[中](README.zh-CN.md) | EN

## Masa.Contrib.Authentication.OpenIdConnect.EntityFrameworkCore

Effect:

Use the Repository to operate the Oidc database

```c#
├── ApiResourceRepository
├── ApiScopeRepository
├── ClientRepository
├── IdentityResourceRepository
├── UserClaimRepository
```

Example：

```C#
Install-Package Masa.Contrib.Authentication.OpenIdConnect.EntityFrameworkCore
```

```C#
builder.Services.AddOidcDbContext(option => option.UseSqlServer("ConnectionString",
    b => b.MigrationsAssembly(migrationsAssembly)));
```

How to use：

```c#
var app = builder.Build();

app.MapGet("/GetClients", async ([FromServices] IClientRepository repository) =>
{
    return await repository.GetListAsync();
});

app.Run();
```
