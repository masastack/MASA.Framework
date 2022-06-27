[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLocking.Medallion

Masa.Contrib.Data.DistributedLocking.Medallion is a distributed lock based on [DistributedLock](https://github.com/madelson/DistributedLock)

### Implementations

- [Azure](../Masa.Contrib.Data.DistributedLocking.Medallion.Azure/README.md)
- [FileSystem](../Masa.Contrib.Data.DistributedLocking.Medallion.FileSystem/README.md)
- [MySql](../Masa.Contrib.Data.DistributedLocking.Medallion.FileSystem/README.md)
- [Oracle](../Masa.Contrib.Data.DistributedLocking.Medallion.FileSystem/README.md)
- [PostgreSql](../Masa.Contrib.Data.DistributedLocking.Medallion.FileSystem/README.md)
- [Redis](../Masa.Contrib.Data.DistributedLocking.Medallion.FileSystem/README.md)
- [SqlServer](../Masa.Contrib.Data.DistributedLocking.Medallion.SqlServer/README.md)
- [WaitHandles](../Masa.Contrib.Data.DistributedLocking.Medallion.FileSystem/README.md)
- [ZooKeeper](../Masa.Contrib.Data.DistributedLocking.Medallion.ZooKeeper/README.md)

### Example:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.Redis// an example of Redis
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
IDistributedLock distributedLock;//Get `IDistributedLock` from DI
using (var lockObj = distributedLock.TryGet("Replace Your Lock Name"))
{
    if (lockObj != null)
    {
        // todo: The code that needs to be executed after acquiring the distributed lock
    }
}
```