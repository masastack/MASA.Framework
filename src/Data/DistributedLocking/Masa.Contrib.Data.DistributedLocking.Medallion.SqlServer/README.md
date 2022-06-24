[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLocking.Medallion.SqlServer

### Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.SqlServer
```

1. Modify `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
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