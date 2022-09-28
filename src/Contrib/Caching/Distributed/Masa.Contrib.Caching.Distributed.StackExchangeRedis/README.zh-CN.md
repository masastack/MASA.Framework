中 | [EN](README.md)

是基于`StackExchangeRedis`开发，提供分布式缓存的能力

* `IDistributedCacheClientFactory`: 分布式缓存工厂，用于创建`IDistributedCacheClient` (Singleton)
* `IDistributedCacheClient`: 分布式缓存客户端 (Singleton)

## Masa.Contrib.Caching.Distributed.StackExchangeRedis

用例：

``` powershell
Install-Package Masa.Contrib.Caching.Distributed.StackExchangeRedis
```

### 入门

#### 用法1:

1. 配置`appsettings.json`

``` C#
{
  "RedisConfig": {
    "Servers": [
      {
        "Host": "localhost",
        "Port": 6379
      }
    ],
    "DefaultDatabase": 3,
    "ConnectionPoolSize": 10
  }
}
```

2. 添加Redis缓存

```C#
builder.Services.AddStackExchangeRedisCache();
```

3. 从DI获取`IDistributedCacheClient`

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

#### 用法2：

1. 添加Redis缓存

```C#
builder.Services.AddStackExchangeRedisCache(new RedisConfigurationOptions()
{
    DefaultDatabase = 1,
    ConnectionPoolSize = 10,
    Servers = new List<RedisServerOptions>()
    {
        new("localhost", 6379)
    }
});
```

2. 从DI获取`IDistributedCacheClient`，并使用相应的方法

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

### 常见问题

1. 绝对过期与滑动过期同时开启时，过期时间怎么算？

当混合使用时，缓存的生命周期有以下三种情况

* 上一次获取或设置缓存时间与本次获取缓存的间隔超过`滑动过期`时间，缓存不存在，获取缓存失败
* 上一次获取或设置缓存时间与本次获取缓存间隔未超过`滑动过期`时间，但当前时间已经超过绝对过期时间，缓存不存在，获取缓存失败
* 上一次获取或设置缓存时间与本次获取缓存间隔未超过`滑动过期`时间，且当前时间小于绝对过期时间，获取缓存成功，生命周期顺延