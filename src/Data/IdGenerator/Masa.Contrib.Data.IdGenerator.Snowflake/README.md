[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.IdGenerator.Snowflake

Masa.Contrib.Data.IdGenerator.Snowflake is an id constructor based on snowflake id, providing a unique identifier of long type

## Example:

1. Install `Masa.Contrib.Data.IdGenerator.Snowflake`

    ````c#
    Install-Package Masa.Contrib.Data.IdGenerator.Snowflake
    ````

2. Using `Masa.Contrib.Data.IdGenerator.Snowflake`

    ```` C#
    builder.Services.AddSnowflake();
    ````

3. Set the value of WorkerId for the current service, add the value of the environment variable `WORKER_ID`, the range is: 0-1023 (2^MaxWorkerId-1)

4. Get Id

    ````
    ISnowflakeGenerator generator;// Get it through DI, or get it through IdGeneratorFactory.SnowflakeGenerator
    generator.Create();//Create a unique id
    ````

### Parameters and FAQs:

* Parameter Description
    * BaseTime: Base time, less than current time (time zone: UTC +0)
      > It is recommended to choose a fixed time that is closer to now. Once used, it cannot be changed (changes may lead to: duplicate id)
    * SequenceBits: serial number, default: 12, support 0-4095 (2^12-1)
      > 4095 requests per worker per millisecond
    * WorkerIdBits: Worker machine id, default: 10, supports 0-1023 machines (2^10-1)
      > By default, it is not supported to be used in k8s clusters. The WorkerId obtained by multiple copies in a Pod is the same, and there may be duplicate IDs.
    * EnableMachineClock: enable clock lock, default: false
      > After enabling the clock lock, the generated id no longer has an absolute relationship with the current time. The generated id takes the time when the project was started as the initial time, and the clock callback after the project runs will not affect the generation of the id
    * The value of WorkerId is obtained from the environment variable `WORKER_ID` by default, if not set, it will return 0
      > When deploying on multiple machines, please ensure that the WorkerId of each service is unique
    * TimestampType: Timestamp type, default: 1 (milliseconds: Milliseconds, seconds: Seconds)
      > When TimestampType is Milliseconds, the maximum length of SequenceBits + WorkerIdBits is 22
      >
      > When TimestampType is Seconds, the maximum length of SequenceBits + WorkerIdBits is 31
* When distributed deployment
    * SupportDistributed: supports distributed deployment, default: false (assigned by the class library provided by WorkerId)
    * HeartbeatInterval: Heartbeat interval, default: 3000ms
      > Used to periodically check the status of the refresh service to ensure that the WorkerId will not be recycled
    * MaxExpirationTime: Maximum expiration time: Default: 10000ms
      > When the refresh service status fails, check that the time difference between the current time and the first refresh service failure exceeds the maximum expiration time, actively give up the current WorkerId, and refuse to provide the service for generating id until a new WorkerId can be obtained and then provide it again Serve
    * MaxCallBackTime: maximum callback time, default: 3000 (milliseconds)
      > When the clock lock is not enabled, if the time callback is less than MaxCallBackTime, the id will be generated again after the waiting time is greater than the last time the id was generated. If it is greater than the maximum callback time, an exception will be thrown

### Performance Testing

1. TimestampType is 1 (milliseconds)
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

2. TimestampType is 2 (seconds)

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

3. TimestampType is 1 (milliseconds), enable clock lock

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

### Notice:

The snowflake id algorithm relies heavily on time. Even after the clock lock is enabled, the project still needs to obtain the current time as the reference time at startup. If the initial acquisition time obtained is an expired time, the generated id may still be repeated.