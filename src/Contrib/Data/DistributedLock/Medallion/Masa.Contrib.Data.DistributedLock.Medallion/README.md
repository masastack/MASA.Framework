[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.DistributedLock.Medallion

Masa.Contrib.Data.DistributedLock.Medallion is a distributed lock based on [DistributedLock](https://github.com/madelson/DistributedLock), it provides the following implementations, you can choose an implementation to use according to your needs

### Implementations

- [Azure](../Masa.Contrib.Data.DistributedLock.Medallion.Azure/README.md)
- [FileSystem](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.md)
- [MySql](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.md)
- [Oracle](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.md)
- [PostgreSql](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.md)
- [Redis](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.md)
- [SqlServer](../Masa.Contrib.Data.DistributedLock.Medallion.SqlServer/README.md)
- [WaitHandles](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.md)
- [ZooKeeper](../Masa.Contrib.Data.DistributedLock.Medallion.ZooKeeper/README.md)

Example:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.Redis //an example of Redis
```

### Get Started

1. Register lock, modify class `Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseRedis("127.0.0.1:6379");
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