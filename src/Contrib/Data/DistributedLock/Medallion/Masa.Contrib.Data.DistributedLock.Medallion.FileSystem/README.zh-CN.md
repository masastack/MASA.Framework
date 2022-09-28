中 | [EN](README.md)

## Masa.Contrib.Data.DistributedLock.Medallion.FileSystem

基于`Masa.Contrib.Data.DistributedLock.Medallion`以及`FileSystem`实现的分布式锁（因为它们是基于文件的，所以这些锁用于在同一台机器上的进程之间进行协调（而不是跨机器））

用例:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.FileSystem
```

### 入门

1. 注册锁，修改类`Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseFileSystem("Replace your directory path");
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

