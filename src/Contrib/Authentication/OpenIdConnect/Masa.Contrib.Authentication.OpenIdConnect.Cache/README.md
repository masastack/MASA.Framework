[中](README.zh-CN.md) | EN

## Masa.Contrib.Authentication.OpenIdConnect.Cache

Effect:

Use the second level cache to operate resources and client data.

```c#
├── ApiResourceCache
├── ApiScopeCache
├── ClientCache
├── IdentityResourceCache
```

Example：

```C#
Install-Package Masa.Contrib.Authentication.OpenIdConnect.Cache
```

```C#
builder.Services.AddOidcCache(nnew RedisConfigurationOptions
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

app.MapGet("/GetClient", async ([FromServices] IClientCache cache) =>
{
    return await cache.GetAsync("clientId");
});

app.Run();
```
