[中](README.zh-CN.md) | EN

It is developed based on `StackExchangeRedis` and provides distributed caching capabilities

* `IDistributedCacheClientFactory`: Distributed Cache Factory for creating `IDistributedCacheClient` (Singleton)
* `IDistributedCacheClient`: Distributed cache client (Singleton)

## Distributed.StackExchangeRedis

Example:

``` powershell
Install-Package Masa.Contrib.Caching.Distributed.StackExchangeRedis
```

### Get Started

#### Usage 1:

1. Add Redis cache

``` C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    var redisConfigurationOptions = new RedisConfigurationOptions()
    {
        DefaultDatabase = 1,
        ConnectionPoolSize = 10,
        Servers = new List<RedisServerOptions>()
        {
            new("localhost", 6379)
        }
    };
    distributedCacheOptions.UseStackExchangeRedisCache(redisConfigurationOptions);
});
```

2. Get `IDistributedCacheClient` from DI and use the corresponding method

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

#### Usage 2:

1. Configure `appsettings.json`

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

2. Add Redis cache

``` C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    // Redis configuration information is obtained through `IOptionsMonitor<RedisConfigurationOptions>`
    distributedCacheOptions.UseStackExchangeRedisCache();
});
```

> By default, the `RedisConfig` node of the local configuration is read, and the configuration supports [`Options Mode`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options), which supports the `IOptionsMonitor<RedisConfigurationOptions>` to get Redis configuration information

3. Get `IDistributedCacheClient` from DI

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

#### Usage 3:

1. Use [Dcc](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.md)

``` C#
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
```

> Please [Reference](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.md)

2 Use the configuration where redis is located

``` C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    var configuration = builder.GetMasaConfiguration().ConfigurationApi.GetSection("{Replace-Your-RedisOptions-AppId}").GetSection("{Replace-Your-RedisOptions-ConfigObjectName}");
    distributedCacheOptions.UseStackExchangeRedisCache(configuration);
});
```

3. Get `IDistributedCacheClient` from DI and use the corresponding method

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

#### Usage 4:

1. Use [Dcc](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.md)

``` C#
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();

    // Use the manual mapping function provided by MasaConfiguration to support the option mode, and support to obtain Redis configuration information through IOptionsMonitor<RedisConfigurationOptions>
    configurationBuilder.UseMasaOptions(option => option.MappingConfigurationApi<RedisConfigurationOptions>("Replace-Your-RedisOptions-AppId", "Replace-Your-RedisOptions-ConfigObjectName", "{Replace-Your-DistributedCacheName}"));
});
```

> Please [Reference](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.md)

2 Use the configuration where redis is located

``` C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    // Redis configuration information is obtained through `IOptionsMonitor<RedisConfigurationOptions>`
    distributedCacheOptions.UseStackExchangeRedisCache());
});
```

> Since the local `RedisConfig` node does not exist, support [Options mode](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options) is skipped by default (avoid providing options with MasaConfiguration [Options mode](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options) repeat)

3. Get `IDistributedCacheClient` from DI and use the corresponding method

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

### common problem

1. When absolute expiration and sliding expiration are enabled at the same time, how is the expiration time calculated?

When mixed use, the cache life cycle has the following three cases

* The interval between the last time of obtaining or setting the cache and the current time of obtaining the cache exceeds the `sliding expiration` time, the cache does not exist, and the access to the cache fails
* The interval between the last acquisition or setting cache time and the current acquisition cache time does not exceed the `sliding expiration` time, but the current time has exceeded the absolute expiration time, the cache does not exist, and the acquisition of the cache fails
* If the interval between the last acquisition or setting cache time and the current acquisition cache time does not exceed the `sliding expiration` time, and the current time is less than the absolute expiration time, the cache acquisition is successful, and the life cycle is extended