[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLock.Medallion.WaitHandles

Distributed lock based on `Masa.Contrib.Data.DistributedLock.Medallion` and `WaitHandles`（Because they are based on [`global WaitHandles in Windows`](https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-createeventa?redirectedfrom=MSDN) distributed locks. This library is only for Windows）

Example:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.WaitHandles
```

1. Register lock, modify class `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseWaitHandles();
});
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

> Does not support cross-machine, only supports coordination between processes on the same machine