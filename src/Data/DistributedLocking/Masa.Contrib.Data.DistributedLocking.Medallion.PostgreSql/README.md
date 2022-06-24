[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLocking.Medallion.PostgreSql

### Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.PostgreSql
```

1. Modify `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseNpgsql("Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity");
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