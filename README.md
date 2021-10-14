# MASA.Contrib

MASA.Contrib is the best practice of MASA.BuildingBlocks

## Structure

```c#
MASA.Contrib
│──solution items
│   ── nuget.config
│──src
│   ├── Data
│   │   ├── MASA.Contrib.Data.Uow.EF                                         Unit of work
│   │   └── MASA.Contribs.Data.Contracts.EF                                  Protocol EF version
│   ├── DDD
│   │   ├── MASA.Contribs.DDD.Domain                                         In-process and cross-process support
│   │   └── MASA.Contribs.DDD.Domain.Repository.EF
│   ├── Dispatcher
│   │   ├── MASA.Contrib.Dispatcher.Events									 In-process event
│   │   ├── MASA.Contrib.Dispatcher.IntegrationEvents.Dapr
│   │   └── MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF			 Cross-process event
│   ├── ReadWriteSpliting
│   │   └── CQRS
│   │   │   └── MASA.Contrib.ReadWriteSpliting.CQRS							 CQRS
│   ├── Service
│   │   └── MASA.Contrib.Service.MinimalAPIs								 Best practices for [MinimalAPI]
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

## Feature

### 1. MinimalAPI

What is [MinimalAPI](https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-6-preview-4/#introducing-minimal-apis)？[Usage introduction](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/Service/MASA.Contrib.Service.MinimalAPIs/README.md)

>  Advantage：
>
>  1.  Classify APIs and add them to different Services to make the Service structure clearer and get rid of running account programming

### 2. EventBus

[Usage introduction](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/Dispatcher/MASA.Contrib.Dispatcher.Events/README.md)

> Advantage：
>
> 1. Arrangement of Handler
> 2. Implement [Saga](https://docs.microsoft.com/zh-cn/azure/architecture/reference-architectures/saga/saga)
> 3. Middleware
> 4. Transaction

> Effect：
>
> 1. Event and Handler decoupling
> 2. Arrangement of Handler
> 3. Implement [Saga](https://docs.microsoft.com/zh-cn/azure/architecture/reference-architectures/saga/saga)
> 4. Middleware
> 5. Transaction

### 3. CQRS

what is[CQRS](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)？[Usage introduction](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/ReadWriteSpliting/CQRS/MASA.Contrib.ReadWriteSpliting.CQRS/README.md)

### 4. IntegrationEventBus

Realize cross-process events based on Dapr。[Usage introduction](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/Dispatcher/MASA.Contrib.Dispatcher.IntegrationEvents.Dapr/README.md)

> Advantage：Use the same transaction to commit the user-defined context and the log to ensure atomicity and consistency

### 5. DomainEventBus

[Usage introduction](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/src/DDD/MASA.Contribs.DDD.Domain/README.md)

> Advantage：
>
> 1. CQRS
> 2. Field Service
> 3. Support domain events (in-process), integrated domain events (cross-process)
> 4. Support the unified sending of field events after being pushed onto the stack

### 6. DDD

[DDD](https://www.likecs.com/default/index/show?id=93970) // todo


### 7. Contracts.EF

Protocol based on EF implementation，[Usage introduction](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/Data/MASA.Contribs.Data.Contracts.EF/README.md)

> Advantage：
>
> 1. Filter deleted information when querying
> 2. Open transaction after query
> 3. Soft delete

```C#
Install-Package MASA.Contribs.Data.Contracts.EF
```

```C#
builder.Services
    .AddUoW<CustomDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
        dbOptions.UseSoftDelete(builder.Services);//Start soft delete
    })
```

> When the entity inherits ISoftware and is deleted, change the delete state to the modified state, and cooperate with the custom Remove operation to achieve soft deletion
> Do not query the data marked as soft deleted when querying
> When combined with EventBus, the transaction is opened after the first CUD, and the transaction rollback is supported when the entire Handler is abnormal.

## Unit testing rules

To ensure the reliability of the entire source code, the unit test coverage is at least 90%

## ☀️ License agreement

[![MASA.Contrib](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](http://gitlab-hz.lonsid.cn/MASA-Stack/Contribs/MASA.Contrib/-/tree/develop/LICENSE)
