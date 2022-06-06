中 | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis

Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis是基于`Masa.Contrib.Data.IdGenerator.Snowflake`
的升级版，支持分布式部署，依赖于Redis提供WorkerId，支持在K8s下部署

## 用例:

1. 安装`Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis`、

    ```c#
    Install-Package Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis
    ```

2. 使用`Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis`

    ``` C#
    builder.Services.AddDistributedSnowflake();
    ```

    > 由于依赖Redis，需使用[Masa.Utils.Caching.Redis](https://github.com/masastack/MASA.Utils/tree/main/src/Caching/Masa.Utils.Caching.Redis)

3. 获取id

    ```
    ISnowflakeGenerator generator;// 通过DI获取，或者通过IdGeneratorFactory.SnowflakeGenerator获取
    generator.Create();//创建唯一id
    ```

### 参数:

* RecycleTime: 回收时间，默认: 120000ms (2min)，当无可用的WorkerId后会尝试从历史使用的WorkerId集合中获取活跃时间超过RecycleTime的WorkerId，并选取距离现在最远的一个WorkerId进行复用
* GetWorkerIdMinInterval: 获取WorkerId的时间间隔，默认: 5000ms (5s)
  > 当前WorkerId可用时，会将WorkerId直接返回，不会有任何限制
  > 当服务刷新WorkerId失败，并持续时间超过指定时间后，会自动释放WorkerId，当再次获取新的Id时，会尝试重新获取新的WorkerId，若最近一次获取WorkerId时间与当前时间小于GetWorkerIdMinInterval时，会被拒绝提供服务
* RefreshTimestampInterval: 默认500ms
  > 选择启用时钟锁后，当获取到下次的时间戳与最近一次的时间戳超过RefreshTimestampInterval时，会将当前的时间戳与WorkerId对应关系保存在Redis中，用于后续继续使用，减少对当前系统时间的依赖