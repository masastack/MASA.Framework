中 | [EN](README.md)

## Masa.Contrib.Authentication.OpenIdConnect.EntityFrameworkCore

作用：

通过Repository操作Oidc数据库

```c#
├── ApiResourceRepository
├── ApiScopeRepository
├── ClientRepository
├── IdentityResourceRepository
├── UserClaimRepository
```

用例：

```C#
Install-Package Masa.Contrib.Authentication.OpenIdConnect.EntityFrameworkCore
```

```C#
builder.Services.AddOidcDbContext(option => option.UseSqlServer("ConnectionString",
    b => b.MigrationsAssembly(migrationsAssembly)));
```

如何使用：

```c#
var app = builder.Build();

app.MapGet("/GetClients", async ([FromServices] IClientRepository repository) =>
{
    return await repository.GetListAsync();
});

app.Run();
```
