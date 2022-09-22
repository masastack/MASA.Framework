[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLock.Medallion.FileSystem

Distributed locks implemented based on `Masa.Contrib.Data.DistributedLock.Medallion` and `FileSystem` (Because they are based on files, these locks are used to coordinate between processes on the same machine (as opposed to across machines) ))

Example:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.FileSystem
```

### Get Started

1. Register lock, modify class `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseFileSystem("Replace your directory path");
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