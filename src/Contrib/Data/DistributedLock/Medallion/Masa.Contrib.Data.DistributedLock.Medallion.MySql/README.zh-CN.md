中 | [EN](README.md)

## Masa.Contrib.Data.DistributedLocking.Medallion.MySql

基于`Masa.Contrib.Data.DistributedLock.Medallion`以及`MySql`实现的分布式锁

用例:

``` powershell
Install-Package Masa.Contrib.Data.DistributedLocking.Medallion.MySql
```

### 入门

1. 注册锁，修改类`Program`

``` C#
builder.Services.AddDistributedLock(medallionBuilder =>
{
    medallionBuilder.UseMySQL("Server=<服务器地址>;port=<端口>;Database=<数据库名称>;Uid=<数据库账号>;Pwd=<数据库密码>");
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

