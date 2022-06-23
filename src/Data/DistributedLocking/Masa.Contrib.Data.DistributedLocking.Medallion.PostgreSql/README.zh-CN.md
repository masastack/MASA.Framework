中 | [EN](README.md)

## Masa.Contrib.Data.DistributedLocking.Medallion.PostgreSql

## 用例:

```c#
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.PostgreSql
```

1. 修改类`Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseNpgsql("Host=myserver;Username=sa;Password=P@ssw0rd;Database=identity");
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

