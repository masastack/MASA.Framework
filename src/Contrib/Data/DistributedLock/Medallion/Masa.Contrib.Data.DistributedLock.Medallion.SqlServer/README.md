[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLock.Medallion.SqlServer

Distributed lock based on `Masa.Contrib.Data.DistributedLock.Medallion` and `SqlServer`

Example:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.SqlServer
```

### Get Started

1. Register lock, modify class `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseSqlServer("server=<your server>;uid=<database account>;pwd=<database password>;database=<your database>");
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