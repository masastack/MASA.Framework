中 | [EN](README.md)

## Masa.Contrib.Data.DistributedLock.Local

基于`SemaphoreSlim`实现的，它不是真正的分布式锁，但它是一个有用的实现

* 可用于在开发环境下
* 不需要考虑调试过程中因为分布式锁而与其他开发冲突的问题（不依赖数据库、Redis、也不依赖网络）

用例:

```c#
Install-Package Masa.Contrib.Data.DistributedLock.Local
```

### 入门

1. 注册锁，修改类`Program`

``` C#
builder.Services.AddLocalDistributedLock();
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