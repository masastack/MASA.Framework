中 | [EN](README.md)

## Masa.Contrib.Data.DistributedLock.Medallion.Oracle

基于`Masa.Contrib.Data.DistributedLock.Medallion`以及`Oracle`实现的分布式锁

用例:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLock.Medallion.Oracle
```

1. 注册锁，修改类`Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseOracle("Data Source=MyOracleDB;Integrated Security=yes;");
});
```

2. 使用锁

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

