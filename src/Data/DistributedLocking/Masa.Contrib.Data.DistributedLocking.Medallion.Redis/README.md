[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLocking.Medallion.Redis

### Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.Redis
```

1. Modify `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseRedis("127.0.0.1:6379");
});
```

2. Use distributed locks

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