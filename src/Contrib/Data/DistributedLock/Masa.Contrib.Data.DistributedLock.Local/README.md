[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLock.Local

Implemented based on `SemaphoreSlim`, it is not really a distributed lock, but it is a useful implementation

* Available in development environment
* No need to consider the problem of conflict with other development due to distributed locks during debugging (does not depend on database, Redis, nor network)

Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLock.Local
```

### Get Started

1. Register lock, modify class `Program`

``` C#
builder.Services.AddLocalDistributedLock();
```

2. Use locks

``` C#
IDistributedLock distributedLock;//Get `IDistributedLock` from DI
using (var lockObj = distributedLock.TryGet("Replace Your Lock Name"))
{
    if (lockObj != null)
    {
        // todo: The code that needs to be executed after acquiring the distributed lock
    }
}
```