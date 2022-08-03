[中](README.zh-CN.md) | EN

## Masa.Contrib.Authentication.OpenIdConnect.Cache.Storage

Effect:

Use IClientStore and IResourceStore get oidc resources and client data.

```c#
├── IClientStore
├── IResourceStore
```

Example：

```C#
Install-Package Masa.Contrib.Authentication.OpenIdConnect.Cache.Storage
```

```C#
builder.Services.AddOidcCacheStorage(nnew RedisConfigurationOptions
{
    Servers = new List<RedisServerOptions>
    {
        new RedisServerOptions
        {
            Host="127.0.0.1",
            Port=6379
        }
    },
    DefaultDatabase = 0,
    Password = "",
});
```

How to use：

```c#
var app = builder.Build();

app.MapGet("/GetClient", async ([FromServices] IClientStore store) =>
{
    return await store.FindClientByIdAsync("clientId");
});

app.Run();
```
