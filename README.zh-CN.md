中 | [EN](README.md)

[![codecov](https://codecov.io/gh/masastack/MASA.Contrib/branch/develop/graph/badge.svg?token=87TPNHUHW2)](https://codecov.io/gh/masastack/MASA.Contrib)

# MASA.Contrib

MASA.Contrib是基于[MASA.BuildingBlocks](https://github.com/masastack/MASA.BuildingBlocks)提供开放, 社区驱动的可重用组件，用于构建网格应用程序。这些组件将被[MaMASAsa Stack](https://github.com/masastack)和[MASA Labs](https://github.com/masalabs)等项目使用。

## 结构

```c#
MASA.Contrib
├── solution items
│   ├── nuget.config
├── src
│   ├── BasicAbility
│   │   ├── Masa.Contrib.BasicAbility.Dcc                          ConfigurationAPI
│   ├── Configuration
│   │   ├── Masa.Contrib.Configuration
│   ├── Data
│   │   ├── Masa.Contrib.Data.UoW.EF                               工作单元
│   │   └── Masa.Contrib.Data.Contracts.EF                        规约EF版
│   ├── Ddd
│   │   ├── Masa.Contrib.Ddd.Domain                               进程内、跨进程都支持
│   │   └── Masa.Contrib.Ddd.Domain.Repository.EF
│   ├── Dispatcher
│   │   ├── Masa.Contrib.Dispatcher.Events                         进程内事件
│   │   ├── Masa.Contrib.Dispatcher.IntegrationEvents.Dapr
│   │   └── Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF 跨进程事件
│   ├── ReadWriteSpliting
│   │   └── Cqrs
│   │   │   └── Masa.Contrib.ReadWriteSpliting.Cqrs                Cqrs
│   ├── Service
│   │   └── Masa.Contrib.Service.MinimalAPIs                       MinimalAPI最佳实践
├── test
│   ├── Masa.Contrib.Dispatcher.Events
│   │   ├── Masa.Contrib.Dispatcher.Events.BenchmarkDotnetTest
│   │   ├── Masa.Contrib.Dispatcher.Events.CheckMethodsParameter.Tests
│   │   ├── Masa.Contrib.Dispatcher.Events.CheckMethodsParameterNotNull.Tests
│   │   ├── Masa.Contrib.Dispatcher.Events.CheckMethodsParameterType.Tests
│   │   ├── Masa.Contrib.Dispatcher.Events.CheckMethodsType.Tests
│   │   ├── Masa.Contrib.Dispatcher.Events.OnlyCancelHandler.Tests
│   │   ├── Masa.Contrib.Dispatcher.Events.CheckMethodsType.Tests
│   │   ├── Masa.Contrib.Dispatcher.Events.Tests
│   ├── Masa.Contrib.Data.UoW.EF.Tests
│   ├── Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests
│   ├── Masa.Contrib.Ddd.Domain.Tests
│   ├── Masa.Contrib.Ddd.Domain.Repository.EF.Tests
```

## 特性

### 1. MinimalAPI

什么是[MinimalAPI](https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-6-preview-4/#introducing-minimal-apis)？[用法介绍](/src/Service/Masa.Contrib.Service.MinimalAPIs/README.zh-CN.md)

>  优势：
>
>  1.  对API进行分类添加到不同的Service，使得Service结构更清晰，摆脱流水账式编程

### 2. EventBus

[用法介绍](/src/Dispatcher/Masa.Contrib.Dispatcher.Events/README.zh-CN.md)

> 优势：
>
> 1. 对Handler的编排
> 2. 实现[Saga](https://docs.microsoft.com/zh-cn/azure/architecture/reference-architectures/saga/saga)
> 3. Middleware
> 4. Transaction

> 作用：
>
> 1. Event与Handler解耦
> 2. 对Handler的编排
> 3. 实现[Saga](https://docs.microsoft.com/zh-cn/azure/architecture/reference-architectures/saga/saga)
> 4. Middleware
> 5. Transaction

### 3. CQRS

什么是[CQRS](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)？[用法介绍](/src/ReadWriteSpliting/Cqrs/Masa.Contrib.ReadWriteSpliting.Cqrs/README.zh-CN.md)

### 4. IntegrationEventBus

基于Dapr实现跨进程的事件。[用法介绍](/src/Dispatcher/Masa.Contrib.Dispatcher.IntegrationEvents.Dapr/README.zh-CN.md)

> 优势：将用户自定义上下文与日志使用同一事务提交，确保原子性、一致性

### 5. DomainEventBus

[用法介绍](/src/Ddd/Masa.Contrib.Ddd.Domain/README.zh-CN.md)

> 优势：
>
> 1. CQRS
> 2. 领域服务
> 3. 支持领域事件（进程内）、集成领域事件（跨进程）
> 4. 支持对领域事件先压栈后统一发送

### 6. DDD

[DDD](https://www.likecs.com/default/index/show?id=93970) // todo


### 7. Contracts.EF

基于EF实现的规约，[用法介绍](src/Data/Masa.Contrib.Data.Contracts.EF/README.zh-CN.md)

> 优势：
>
> 1. 查询的时候过滤已删除的信息
> 2. 软删除

```C#
Install-Package Masa.Contrib.Data.Contracts.EF
```

```C#
builder.Services.AddEventBus(options => {
    options.UseUoW<CustomDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
        dbOptions.UseSoftDelete(builder.Services);//启动软删除
    });
});
```

> 当实体继承ISoftware，且被删除时，将删除状态改为修改状态，并配合自定义Remove操作，实现软删除
> 支持查询的时候不查询被标记软删除的数据
> 与EventBus结合使用时，做到了第一次CUD后开启事务，当整个Handler出现异常后支持事务回滚

### 8. Masa.Contrib.Configuration

重定义Configuration，支持Local、ConfigurationAPI节点的管理，结合IOptions、IOptionsMonitor完成配置的获取以及配置的更新订阅 [Local用法介绍](src/Configuration/Masa.Contrib.Configuration/README.zh-CN.md) 、[Dcc用法介绍](src/BasicAbility/Masa.Contrib.BasicAbility.Dcc/README.zh-CN.md)

## 单元测试规则

为确保整个源码的可靠性，单元测试覆盖率最低为90%

## ☀️ 授权协议

[![MASA.Contrib](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](/LICENSE.txt)

