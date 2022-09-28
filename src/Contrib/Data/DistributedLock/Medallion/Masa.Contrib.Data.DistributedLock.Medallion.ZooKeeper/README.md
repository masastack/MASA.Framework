[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLock.Medallion.ZooKeeper

Distributed lock based on `Masa.Contrib.Data.DistributedLock.Medallion` and `ZooKeeper`

Example:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.SqlServer
```

### Get Started

1. Register lock, modify class `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseZooKeeper("Replace your ZooKeeper connectionString");
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