中 | [EN](README.md)

## Masa.Contrib.Data.DistributedLock.Medallion

Masa.Contrib.Data.DistributedLock.Medallion是基于[DistributedLock](https://github.com/madelson/DistributedLock)的一个分布式锁

## 实现

- [Azure](../Masa.Contrib.Data.DistributedLock.Medallion.Azure/README.zh-CN.md)
- [FileSystem](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.zh-CN.md)
- [MySql](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.zh-CN.md)
- [Oracle](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.zh-CN.md)
- [PostgreSql](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.zh-CN.md)
- [Redis](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.zh-CN.md)
- [SqlServer](../Masa.Contrib.Data.DistributedLock.Medallion.SqlServer/README.zh-CN.md)
- [WaitHandles](../Masa.Contrib.Data.DistributedLock.Medallion.FileSystem/README.zh-CN.md)
- [ZooKeeper](../Masa.Contrib.Data.DistributedLock.Medallion.ZooKeeper/README.zh-CN.md)

## 用例:

```c#
Install-Package Masa.Contrib.Data.DistributedLock.Medallion
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.Redis//以Redis举例
```

1. 修改类`Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseRedis("127.0.0.1:6379");
});
```

2. 使用分布式锁

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

