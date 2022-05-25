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

4. Get id

    ````
    IIdGenerator idGenerator;// Get through DI
    idGenerator.Generate();//Create a unique id
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
    * TimestampType: timestamp type, default: 1 (milliseconds: 1, seconds: 2)
      > When TimestampType is 1, the maximum length of SequenceBits + WorkerIdBits is 22
      > When TimestampType is 2, the maximum length of SequenceBits + WorkerIdBits is 31
* When distributed deployment
    * SupportDistributed: supports distributed deployment, default: false (assigned by the class library provided by WorkerId)
    * HeartbeatInterval: Heartbeat interval, default: 3000ms
      > Used to periodically check the status of the refresh service to ensure that the WorkerId will not be recycled
    * MaxExpirationTime: Maximum expiration time: Default: 10000ms
      > When the refresh service status fails, check that the time difference between the current time and the first refresh service failure exceeds the maximum expiration time, actively give up the current WorkerId, and refuse to provide the service for generating id until a new WorkerId can be obtained and then provide it again Serve

### Notice:

The snowflake id algorithm relies heavily on time. Even after the clock lock is enabled, the project still needs to obtain the current time as the reference time at startup. If the initial acquisition time obtained is an expired time, the generated id may still be repeated.