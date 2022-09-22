[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis

Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis is based on `Masa.Contrib.Data.IdGenerator.Snowflake`
The upgraded version supports distributed deployment, relies on Redis to provide WorkerId, and supports deployment under K8s

Example:

1. Install `Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis`,

    ``` c#
    Install-Package Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis
    ```

2. Use `Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis`

    ``` C#
    builder.Services.AddSnowflake(option => option.UseRedis());
    ```

    > Due to the dependency on Redis, [Masa.Utils.Caching.Redis](https://github.com/masastack/MASA.Utils/tree/main/src/Caching/Masa.Utils.Caching.Redis)

3. Get id

    ``` C#
    ISnowflakeGenerator generator;// Get it through DI, or get it through IdGeneratorFactory.SnowflakeGenerator
    generator.NewId();//Create a unique id
    ```

### Parameters:

* IdleTimeOut: Idle recycling time, default: 120000ms (2min), when there is no available WorkerId, it will try to obtain the WorkerId whose active time exceeds IdleTimeOut from the historically used WorkerId collection, and select the one that is farthest away from the current WorkerId for reuse
* GetWorkerIdMinInterval: Time interval for getting WorkerId, default: 5000ms (5s)
  > When the current WorkerId is available, the WorkerId will be returned directly without any restrictions
  > When the service fails to refresh the WorkerId and the duration exceeds the specified time, the WorkerId will be automatically released. When a new Id is obtained again, it will try to obtain a new WorkerId again. If the latest WorkerId obtained and the current time are less than GetWorkerIdMinInterval, the Denied service
* RefreshTimestampInterval: default 500ms
  > After selecting to enable the clock lock, when the obtained next time stamp and the most recent time stamp exceed RefreshTimestampInterval, the corresponding relationship between the current time stamp and WorkerId will be saved in Redis for subsequent use, reducing the need for the current system time dependence