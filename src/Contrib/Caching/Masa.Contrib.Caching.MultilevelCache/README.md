[中](README.zh-CN.md) | EN

Currently provides the ability of multi-level cache, which is composed of memory cache and distributed cache

* `IMultilevelCacheClientFactory`: Multilevel cache factory for creating `IDistributedCacheClient` (Singleton)
* `IDistributedCacheClient`: multi-level cache client (Singleton)

## Masa.Contrib.Caching.MultilevelCache

Example:

```` powershell
Install-Package Masa.Contrib.Caching.MultilevelCache
````

### Get Started

#### Usage 1:

1. Configure `appsettings.json`

``` C#
{
  // Redis distributed cache configuration
  "RedisConfig": {
    "Servers": [
      {
        "Host": "localhost",
        "Port": 6379
      }
    ],
    "DefaultDatabase": 3
  },

  // Multi-level cache global configuration, optional
  "MultilevelCache": {
    "SubscribeKeyPrefix": "masa",//The default subscriber key prefix, used for splicing channels
    "SubscribeKeyType": 3, //Default subscriber key type, default ValueTypeFullNameAndKey, used for splicing channels
    "CacheEntryOptions": {
      "AbsoluteExpirationRelativeToNow": "00:00:30",//Absolute expiration time (from the current time)
      "SlidingExpiration": "00:00:50"//Sliding expiration time (from the current time)
    }
  }
}
```

2. Add multi-level cache

``` C#
builder.Services.AddMultilevelCache(distributedCacheOptions =>
{
    distributedCacheOptions.UseStackExchangeRedisCache();
});
```

3. Get `IMultilevelCacheClient` from DI

``` C#
string key = "test_1";
multilevelCacheClient.Set(key, "test_content");
```

#### Usage 2:

1. Add multi-level cache

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
       .AddMultilevelCache(
           distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(redisConfigurationOptions),
           multilevelCacheOptions =>
           {
               multilevelCacheOptions.SubscribeKeyPrefix = "masa";
               multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.ValueTypeFullNameAndKey;
           });
```

2. Get `IMultilevelCacheClient` from DI and use the corresponding method

```` C#
string key = "test_1";
multilevelCacheClient.Set(key, "test_content");
````