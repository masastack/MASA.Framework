[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLock.Local

Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLock.Local
```

1. Modify the class `Program`

``` C#
builder.Services.AddLocalDistributedLock();
```

2. Use distributed locks

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