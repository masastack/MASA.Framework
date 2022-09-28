中 | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.Snowflake

Masa.Contrib.Data.IdGenerator.Snowflake是一个基于雪花id的id构造器，提供long类型的唯一标识

用例:

``` powershell
Install-Package Masa.Contrib.Data.IdGenerator.Snowflake
```

### 入门

1. 注册雪花id构造器，修改`Program.cs`

``` C#
builder.Services.AddSnowflake();
```

2. 为当前服务设置WorkerId的值，添加环境变量`WORKER_ID`的值，其范围为: 0-1023 (2^MaxWorkerId-1)，默认: 0

3. 获取Id

```
ISnowflakeGenerator generator;// 通过DI获取
generator.NewId();//创建唯一id
```

> 或通过`MasaApp.GetRequiredService<ISnowflakeGenerator>().NewId()`获取

### 参数及常见问题:

* 参数说明
    * BaseTime: 基准时间，小于当前时间（时区：UTC +0）
      > 建议选用现在更近的固定时间，一经使用，不可更变（更改可能导致: 重复id）
    * SequenceBits: 序列号, 默认: 12，支持0-4095 (2^12-1)
      > 每毫秒每个工作机器最多产生4095个请求
    * WorkerIdBits: 工作机器id，默认: 10，支持0-1023个机器 (2^10-1)
      > 默认不支持在k8s集群中使用，在一个Pod中多副本获取到的WorkerId是一样的，可能会出现重复id
    * EnableMachineClock: 启用时钟锁，默认: false
      > 启用时钟锁后，生成的id不再与当前时间有绝对关系，生成的id以项目启动时的时间作为初始时间，项目运行后时钟回拨不会影响id的生成
    * WorkerId的值默认从环境变量`WORKER_ID`中获取，如未设置则会返回0
      > 多机部署时请确保每个服务的WorkerId是唯一的
    * TimestampType: 时间戳类型，默认: 1 (毫秒: Milliseconds, 秒: Seconds)
      > TimestampType为Milliseconds时，SequenceBits + WorkerIdBits 最大长度为22
      >
      > TimestampType为Seconds时，SequenceBits + WorkerIdBits 最大长度为31
    * MaxCallBackTime: 最大回拨时间，默认: 3000 (毫秒)
      > 当不启用时钟锁时，如果出现时间回拨小于MaxCallBackTime，则会等待时间大于最后一次生成id的时间后，再次生成id，如果大于最大回拨时间，则会抛出异常

* 分布式部署时
    * SupportDistributed: 支持分布式部署，默认: false (由WorkerId的提供类库赋值)
    * HeartbeatInterval: 心跳周期，默认: 3000ms
      > 用于定期检查刷新服务的状态，确保WorkerId不会被回收
    * MaxExpirationTime: 最大过期时间: 默认: 10000ms
      > 当刷新服务状态失败时，检查当前时间与第一次刷新服务失败的时间差超过最大过期时间后，主动放弃当前的WorkerId，并拒绝提供生成id的服务，直到可以获取到新的WokerId后再次提供服务

### 性能测试

1. TimestampType为1（毫秒）

`BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1023 (21H1/May2021Update)
11th Gen Intel Core i7-11700 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-preview.4.22252.9
[Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT DEBUG
Job-JPQDWN : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-BKJUSV : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-UGZQME : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT`

`Runtime=.NET 6.0  RunStrategy=ColdStart`

|                 Method |        Job | IterationCount |       Mean |     Error |     StdDev |     Median |        Min |          Max |
|----------------------- |----------- |--------------- |-----------:|----------:|-----------:|-----------:|-----------:|-------------:|
| SnowflakeByMillisecond | Job-JPQDWN |           1000 | 2,096.1 ns | 519.98 ns | 4,982.3 ns | 1,900.0 ns | 1,000.0 ns | 156,600.0 ns |
| SnowflakeByMillisecond | Job-BKJUSV |          10000 |   934.0 ns |  58.44 ns | 1,775.5 ns |   500.0 ns |   200.0 ns | 161,900.0 ns |
| SnowflakeByMillisecond | Job-UGZQME |         100000 |   474.6 ns |   5.54 ns |   532.8 ns |   400.0 ns |   200.0 ns | 140,500.0 ns |

2. TimestampType为2（秒）

`BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1023 (21H1/May2021Update)
11th Gen Intel Core i7-11700 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-preview.4.22252.9
[Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-RVUKKG : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-JAUDMW : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-LOMSTK : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT`

`Runtime=.NET 6.0  RunStrategy=ColdStart`

|            Method |        Job | IterationCount |      Mean |      Error |       StdDev |    Median |       Min |          Max |
|------------------ |----------- |--------------- |----------:|-----------:|-------------:|----------:|----------:|-------------:|
| SnowflakeBySecond | Job-RVUKKG |           1000 |  1.882 us |  0.5182 us |     4.965 us | 1.5000 us | 0.9000 us |     158.0 us |
| SnowflakeBySecond | Job-JAUDMW |          10000 | 11.505 us | 35.1131 us | 1,066.781 us | 0.4000 us | 0.3000 us | 106,678.8 us |
| SnowflakeBySecond | Job-LOMSTK |         100000 | 22.097 us | 15.0311 us | 1,444.484 us | 0.4000 us | 0.2000 us | 118,139.7 us |

3. TimestampType为1（毫秒）、启用时钟锁

`BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1023 (21H1/May2021Update)
11th Gen Intel Core i7-11700 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-preview.4.22252.9
[Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-BBZSDR : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-NUSWYF : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
Job-FYICRN : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT`

`Runtime=.NET 6.0  RunStrategy=ColdStart`

|                    Method |        Job | IterationCount |       Mean |     Error |     StdDev |     Median |         Min |          Max |
|-------------------------- |----------- |--------------- |-----------:|----------:|-----------:|-----------:|------------:|-------------:|
| MachineClockByMillisecond | Job-BBZSDR |           1000 | 1,502.0 ns | 498.35 ns | 4,775.1 ns | 1,100.0 ns | 700.0000 ns | 151,600.0 ns |
| MachineClockByMillisecond | Job-NUSWYF |          10000 |   602.0 ns |  54.76 ns | 1,663.7 ns |   200.0 ns | 100.0000 ns | 145,400.0 ns |
| MachineClockByMillisecond | Job-FYICRN |         100000 |   269.8 ns |   5.64 ns |   542.4 ns |   200.0 ns |   0.0000 ns | 140,900.0 ns |

### 注意：

雪花id算法严重依赖时间，哪怕是启用时钟锁后，项目在启动时仍然需要获取一次当前时间作为基准时间，如果获取到的初始获取时间为已经过期的时间，那生成的id仍然有重复的可能
