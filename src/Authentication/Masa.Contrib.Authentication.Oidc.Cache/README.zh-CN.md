中 | [EN](README.md)

## Masa.Contrib.Authentication.Oidc.Cache

作用：

使用二级缓存来操作资源和客户端数据。

```c#
├── ApiResourceCache
├── ApiScopeCache
├── ClientCache
├── IdentityResourceCache
```

用例：

```C#
Install-Package Masa.Contrib.Authentication.Oidc.Cache
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

如何使用：

```c#
var app = builder.Build();

app.MapGet("/GetClient", async ([FromServices] IClientCache cache) => 
{
    return await cache.GetAsync("clientId");
});

app.Run();
```
