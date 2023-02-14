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

#### 用法1：

1. 添加Redis缓存

```C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    var redisConfigurationOptions = new RedisConfigurationOptions()
    {
        DefaultDatabase = 1,
        Servers = new List<RedisServerOptions>()
        {
            new("localhost", 6379)
        }
    };
    distributedCacheOptions.UseStackExchangeRedisCache(redisConfigurationOptions);
});
```

2. 从DI获取`IDistributedCacheClient`，并使用相应的方法

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

#### 用法2:

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
    "DefaultDatabase": 3
  }
}
```

2. 添加Redis缓存

```C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    // Redis配置信息是通过 IOptionsMonitor<RedisConfigurationOptions> 来获取
    distributedCacheOptions.UseStackExchangeRedisCache();
});
```

> 默认读取本地配置的`RedisConfig`节点，并配置支持[`选项模式`](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/options)，支持了通过`IOptionsMonitor<RedisConfigurationOptions>`来获取Redis配置的信息

3. 从DI获取`IDistributedCacheClient`

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

#### 用法3：

1. 使用[Dcc](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.zh-CN.md)

```C#
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
```

> Dcc配置使用请[参考](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.zh-CN.md)

2 使用redis所在的配置

```C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    var configuration = builder.GetMasaConfiguration().ConfigurationApi.GetSection("{Replace-Your-RedisOptions-AppId}").GetSection("{Replace-Your-RedisOptions-ConfigObjectName}");
    distributedCacheOptions.UseStackExchangeRedisCache(configuration);
});
```

3. 从DI获取`IDistributedCacheClient`，并使用相应的方法

``` C#
string key = "test_1";
distributedCacheClient.Set(key, "test_content");
```

#### 用法4：

1. 使用[Dcc](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.zh-CN.md)

```C#
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();

    // 使用 MasaConfiguration提供的手动映射功能，使其支持 选项模式，支持通过 IOptionsMonitor<RedisConfigurationOptions> 来获取Redis配置的信息
    configurationBuilder.UseMasaOptions(option => option.MappingConfigurationApi<RedisConfigurationOptions>("Replace-Your-RedisOptions-AppId", "Replace-Your-RedisOptions-ConfigObjectName", "{Replace-Your-DistributedCacheName}"));
});
```

> Dcc配置使用请[参考](../../../Configuration/ConfigurationApi/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.zh-CN.md)

2 使用redis所在的配置

``` C#
builder.Services.AddDistributedCache(distributedCacheOptions =>
{
    // Redis配置信息是通过`IOptionsMonitor<RedisConfigurationOptions>`来获取
    distributedCacheOptions.UseStackExchangeRedisCache());
});
```

> 由于本地`RedisConfig`节点不存在，默认会跳过支持[选项模式](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/options) (避免与MasaConfiguration提供[选项模式](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/options)重复)

3. 从DI获取`IDistributedCacheClient`，并使用相应的方法

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