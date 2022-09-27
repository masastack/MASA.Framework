中 | [EN](README.md)

## Masa.Contrib.Data.DistributedLock.Medallion.WaitHandles

基于`Masa.Contrib.Data.DistributedLock.Medallion`以及`WaitHandles`实现的分布式锁（因为它们是基于[`Windows 中全局 WaitHandles`](https://learn.microsoft.com/zh-cn/windows/win32/api/synchapi/nf-synchapi-createeventa?redirectedfrom=MSDN)分布式锁。此库仅适用于 Windows）

用例:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.WaitHandles
```

### 入门

1. 注册锁，修改类`Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseWaitHandles();
});
```

2. 使用锁

``` C#
IDistributedLock distributedLock;//从DI获取`IDistributedLock`
using (var lockObj = distributedLock.TryGet("Replace Your Lock Name"))
{
    if (lockObj != null)
    {
        // todo: The code that needs to be executed after acquiring the distributed lock
    }
}
```

> 不支持跨机器，仅支持同一台机器上进程之间进行协调