[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLocking.Local

## Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Local
```

1. Modify the class `Program`

``` C#
builder.Services.AddLocalDistributedLock();
```

2. Use distributed locks

``` C#
IDistributedLock distributedLock;//从DI获取`IDistributedLock`
using (var lockObj = distributedLock.TryGet("Replace You Lock Name"))
{
    if (lockObj != null)
    {
        // todo: The code that needs to be executed after acquiring the distributed lock
    }
}
```