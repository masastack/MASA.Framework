﻿中 | [EN](README.md)

# MASA.Contrib

MASA.BuildingBlocks最佳实践

## 结构

```c#
MASA.Contrib
│──solution items
│   ── nuget.config
│──src
│   ├── Data
│   │   ├── MASA.Contrib.Data.Uow.EF                                         工作单元
│   │   └── MASA.Contribs.Data.Contracts.EF                                  规约EF版
│   ├── DDD
│   │   ├── MASA.Contribs.DDD.Domain                                         进程内、跨进程都支持
│   │   └── MASA.Contribs.DDD.Domain.Repository.EF
│   ├── Dispatcher
│   │   ├── MASA.Contrib.Dispatcher.Events									 进程内事件
│   │   ├── MASA.Contrib.Dispatcher.IntegrationEvents.Dapr
│   │   └── MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF			 跨进程事件
│   ├── ReadWriteSpliting
│   │   └── CQRS
│   │   │   └── MASA.Contrib.ReadWriteSpliting.CQRS							 CQRS
│   ├── Service
│   │   └── MASA.Contrib.Service.MinimalAPIs								 MinimalAPI最佳实践
│──test
│   ├── MASA.Contrib.Dispatcher.Events
│   │   ├── MASA.Contrib.Dispatcher.Events.BenchmarkDotnetTest
│   │   ├── MASA.Contrib.Dispatcher.Events.CheckMethodsParameter.Tests
│   │   ├── MASA.Contrib.Dispatcher.Events.CheckMethodsParameterNotNull.Tests
│   │   ├── MASA.Contrib.Dispatcher.Events.CheckMethodsParameterType.Tests
│   │   ├── MASA.Contrib.Dispatcher.Events.CheckMethodsType.Tests
│   │   ├── MASA.Contrib.Dispatcher.Events.OnlyCancelHandler.Tests
│   │   ├── MASA.Contrib.Dispatcher.Events.CheckMethodsType.Tests
│   │   ├── MASA.Contrib.Dispatcher.Events.Tests
│   ├── MASA.Contrib.Data.Uow.EF.Tests
│   ├── MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests
│   ├── MASA.Contribs.DDD.Domain.Tests
│   ├── MASA.Contribs.DDD.Domain.Repository.EF.Tests
```

## 特性

### 1. MinimalAPI

什么是[MinimalAPI](https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-6-preview-4/#introducing-minimal-apis)？[用法介绍](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/Service/MASA.Contrib.Service.MinimalAPIs/README.zh-cn.md)

>  优势：
>
>  1.  对API进行分类添加到不同的Service，使得Service结构更清晰，摆脱流水账式编程

### 2. EventBus

[用法介绍](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/Dispatcher/MASA.Contrib.Dispatcher.Events/README.zh-cn.md)

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

什么是[CQRS](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)？[用法介绍](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/ReadWriteSpliting/CQRS/MASA.Contrib.ReadWriteSpliting.CQRS/README.zh-cn.md)

### 4. IntegrationEventBus

基于Dapr实现跨进程的事件。[用法介绍](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/Dispatcher/MASA.Contrib.Dispatcher.IntegrationEvents.Dapr/README.zh-cn.md)

> 优势：将用户自定义上下文与日志使用同一事务提交，确保原子性、一致性

### 5. DomainEventBus

[用法介绍](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/DDD/MASA.Contribs.DDD.Domain/README.zh-cn.md)

> 优势：
>
> 2. CQRS
> 3. 领域服务
> 4. 支持领域事件（进程内）、集成领域事件（跨进程）
> 4. 支持对领域事件先压栈后统一发送

### 6. DDD

[DDD](https://www.likecs.com/default/index/show?id=93970) // todo


### 7. Contracts.EF

基于EF实现的规约，[用法介绍](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/Data/MASA.Contribs.Data.Contracts.EF/README.zh-cn.md)

> 优势：
>
> 1. 查询的时候过滤已删除的信息
> 2. 查询后开启事务
> 3. 软删除

```C#
Install-Package MASA.Contribs.Data.Contracts.EF
```

```C#
builder.Services
    .AddUoW<CustomDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
        dbOptions.UseSoftDelete(builder.Services);//启动软删除
    })
```

> 当实体继承ISoftware，且被删除时，将删除状态改为修改状态，并配合自定义Remove操作，实现软删除
> 支持查询的时候不查询被标记软删除的数据
> 与EventBus结合使用时，做到了第一次CUD后开启事务，当整个Handler出现异常后支持事务回滚

## 单元测试规则

为确保整个源码的可靠性，单元测试覆盖率最低为90%

## ☀️ 授权协议

[![MASA.Contrib](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/LICENSE)

