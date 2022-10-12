中 | [EN](README.md)

当前提供了多级缓存的能力，它是由内存缓存与分布式缓存构成

* `IMultilevelCacheClientFactory`: 多级缓存工厂，用于创建`IDistributedCacheClient` (Singleton)
* `IDistributedCacheClient`: 多级缓存客户端 (Singleton)

## Masa.Contrib.Caching.MultilevelCache

用例：

``` powershell
Install-Package Masa.Contrib.Caching.Distributed.StackExchangeRedis //用于提供分布式缓存能力，这里以Redis为例
Install-Package Masa.Contrib.Caching.MultilevelCache
```
### 入门

#### 用法1:

1. 配置`appsettings.json`

``` appsettings.json
{
  // Redis分布式缓存配置
  "RedisConfig": {
    "Servers": [
      {
        "Host": "localhost",
        "Port": 6379
      }
    ],
    "DefaultDatabase": 3
  },

  // 多级缓存全局配置，非必填
  "MultilevelCache": {
    "SubscribeKeyPrefix": "masa",//默认订阅方key前缀，用于拼接channel
    "SubscribeKeyType": 3, //默认订阅方key的类型，默认ValueTypeFullNameAndKey，用于拼接channel
    "CacheEntryOptions": {
      "AbsoluteExpirationRelativeToNow": "00:00:30",//绝对过期时长（距当前时间）
      "SlidingExpiration": "00:00:50"//滑动过期时长（距当前时间）
    }
  }
}
```

2. 添加多级缓存

``` C#
builder.Services.AddStackExchangeRedisCache()
                .AddMultilevelCache();
```

3. 从DI获取`IMultilevelCacheClient`

``` C#
string key = "test_1";
multilevelCacheClient.Set(key, "test_content");
```

#### 用法2：

1. 添加多级缓存

``` C#
var redisConfigurationOptions = new RedisConfigurationOptions()
{
    DefaultDatabase = 1,
    Servers = new List<RedisServerOptions>()
    {
        new("localhost", 6379)
    }
};
builder.Services
    .AddStackExchangeRedisCache(redisConfigurationOptions)
    .AddMultilevelCache(new MultilevelCacheOptions()
    {
        SubscribeKeyPrefix = "masa",
        SubscribeKeyType = SubscribeKeyType.ValueTypeFullNameAndKey
    });
```

2. 从DI获取`IMultilevelCacheClient`，并使用相应的方法

``` C#
string key = "test_1";
multilevelCacheClient.Set(key, "test_content");
```