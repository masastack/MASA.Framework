[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLocking.Medallion.WaitHandles

### Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.WaitHandles
```

1. Modify `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseWaitHandles();
});
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

> Does not support cross-machine, only supports coordination between processes on the same machine