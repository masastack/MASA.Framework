中 | [EN](README.md)

## Masa.Contrib.Authentication.Oidc.Cache.Storag

作用：

通过IClientStore和IResourceStore获取资源和客户端的相关数据

```c#
├── IClientStore
├── IResourceStore
```

用例：

```C#
Install-Package Masa.Contrib.Authentication.Oidc.Cache.Storag
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

如何使用：

```c#
var app = builder.Build();

app.MapGet("/GetClient", async ([FromServices] IClientStore store) => 
{
    return await store.FindClientByIdAsync("clientId");
});

app.Run();
```
