[中](README.zh-CN.md) | EN

It is developed based on `StackExchangeRedis` and provides distributed caching capabilities

* `IDistributedCacheClientFactory`: Distributed Cache Factory for creating `IDistributedCacheClient` (Singleton)
* `IDistributedCacheClient`: Distributed cache client (Singleton)

## Distributed.StackExchangeRedis

Example:

````C#
Install-Package Masa.Contrib.Caching.Distributed.StackExchangeRedis
````

### Usage 1:

1. Configure appsettings.json

```` C#
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
````

2. Add Redis cache

````C#
builder.Services.AddStackExchangeRedisCache();
````

3. Get `IDistributedCacheClient` from DI

    ```` C#
    string key = "test_1";
    distributedCacheClient.Set(key, "test_content");
    ````

### Usage 2:

1. Add Redis cache

````C#
builder.Services.AddStackExchangeRedisCache(new RedisConfigurationOptions()
{
    DefaultDatabase = 1,
    ConnectionPoolSize = 10,
    Servers = new List<RedisServerOptions>()
    {
        new("localhost", 6379)
    }
});
````

2. Get `IDistributedCacheClient` from DI and use the corresponding method

    ```` C#
    string key = "test_1";
    distributedCacheClient.Set(key, "test_content");
    ````

## common problem

1. When absolute expiration and sliding expiration are enabled at the same time, how is the expiration time calculated?

When mixed use, the cache life cycle has the following three cases

* The interval between the last time of obtaining or setting the cache and the current time of obtaining the cache exceeds the `sliding expiration` time, the cache does not exist, and the access to the cache fails
* The interval between the last acquisition or setting cache time and the current acquisition cache time does not exceed the `sliding expiration` time, but the current time has exceeded the absolute expiration time, the cache does not exist, and the acquisition of the cache fails
* If the interval between the last acquisition or setting cache time and the current acquisition cache time does not exceed the `sliding expiration` time, and the current time is less than the absolute expiration time, the cache acquisition is successful, and the life cycle is extended