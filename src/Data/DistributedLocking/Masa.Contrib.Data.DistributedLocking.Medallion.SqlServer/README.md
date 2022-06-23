[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLocking.Medallion.SqlServer

### Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.SqlServer
```

1. 修改类`Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
});
```

2. 使用分布式锁

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