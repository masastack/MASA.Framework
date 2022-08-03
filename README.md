[中](README.zh-CN.md) | EN

[![codecov](https://codecov.io/gh/masastack/MASA.Contrib/branch/main/graph/badge.svg?token=87TPNHUHW2)](https://codecov.io/gh/masastack/MASA.Contrib)

# MASA.Contrib

The purpose of MASA.Contrib is based on [MASA.BuildingBlocks](https://github.com/masastack/MASA.BuildingBlocks) to provide open, community driven reusable components for building mesh applications.  These components will be used by the [MASA Stack](https://github.com/masastack) and [MASA Labs](https://github.com/masalabs) projects.



## Roadmap
* [Release Notes](https://github.com/masastack/MASA.Contrib/releases)
* [Latest Roadmap](https://github.com/masastack/MASA.Contrib/issues/42)



## Features
* AutoComplete: make searching easier
  * [ElasticSearch](./src/SearchEngine/Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch/README.md)
* Configuration: Configuration Center
  * [Configuration core, provide local configuration](./src/Configuration/Masa.Contrib.Configuration/README.md)
  * ConfigurationAPI
    * [Dcc](./src/Configuration/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.md)
* [CQRS](./src/ReadWriteSpliting/Cqrs/Masa.Contrib.ReadWriteSpliting.Cqrs/README.md)
* Data:
  * [EntityFrameworkCore](./src/Data/Masa.Contrib.Data.EntityFrameworkCore/README.md)
    * [SqlServer](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.SqlServer/README.md)
    * [Pomelo.MySql](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Pomelo.MySql/README.md): Recommend
    * [MySql](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.MySql/README.md)
    * [Sqlite](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Sqlite/README.md)
    * [Cosmos](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Cosmos/README.md)
    * [InMemory](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.InMemory/README.md)
    * [Oracle](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Oracle/README.md)
    * [PostgreSql](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.PostgreSql/README.md)
  * [Data.Contracts.EF](./src/Data/Masa.Contrib.Data.Contracts.EntityFrameworkCore/): data protocol
  * UoW: unit of work
    * [EFCore](./src/Data/Masa.Contrib.Data.UoW.EF/README.md)
  * IdGenerator: Unique ID generator
     * [NormalGuid](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.NormalGuid/README.md): Normal Guid
     * [SequentialGuid](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.SequentialGuid/README.md): Sequential Guid
     * [Snowflake](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.Snowflake/README.md): Snowflake id
     * [Snowflake.Distributed.Redis](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis/README.md): Distributed snowflake id
  * Mapping: object mapping
     * [Mapster](./src/Data/Mapping/Masa.Contrib.Data.Mapping.Mapster/README.md)
* [DDD](./src/Ddd/Masa.Contrib.Ddd.Domain/README.md)
  * [Ddd.Domain.Repository.EF](./src/Ddd/Masa.Contrib.Ddd.Domain.Repository.EntityFrameworkCore/README.md): Provide warehousing services
* Dispatcher
  * [EventBus](./src/Dispatcher/Masa.Contrib.Dispatcher.Events/README.md): In-process events
  * [IntegrationEventBus](./src/Dispatcher/Masa.Contrib.Dispatcher.IntegrationEvents.Dapr/README.md): Cross-process events
    * [IntegrationEvents.EventLogs.EF](./src/Dispatcher/Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore/README.md): Provides message management services for cross-process events
* Isolation: Support physical isolation, logical isolation
  * [UoW.EF](./src/Isolation/Masa.Contrib.Isolation.UoW.EntityFrameworkCore/README.md)
  * [MultiEnvironment](./src/Isolation/Masa.Contrib.Isolation.MultiEnvironment/README.md): MultiEnvironment
  * [MultiTenant](./src/Isolation/Masa.Contrib.Isolation.MultiTenant/README.md): Multi-tenancy
* [MinimalAPI](./src/Service/Masa.Contrib.Service.Mini~~~~malAPIs/README.md): Support API classification aggregation similar to Controller
* Storage: cloud storage
  * [Aliyun Storage](./src/Storage/Masa.Contrib.Storage.ObjectStorage.Aliyun/README.md)
* Operational capacity
  * [Auth](./src/StackSdks/Masa.Contrib.StackSdks.Auth/README.md): Authentication and Authorization
  * [Dcc](./src/StackSdks/Masa.Contrib.StackSdks.Dcc/README.md): Distributed Configuration Center
  * [PM](./src/StackSdks/Masa.Contrib.StackSdks.Pm/README.md): Project Management
  * [Scheduler](./src/StackSdks/Masa.Contrib.StackSdks.Scheduler/README.md): Distributed Scheduler
  * [TSC](./src/StackSdks/Masa.Contrib.StackSdks.Tsc/README.md): Troubleshooting Console



## How to clone
````
git clone --recursive https://github.com/masastack/MASA.Contrib.git
````



## How to contribute

1. Fork & Clone
2. Create Feature_xxx branch
3. Commit with commit message, like `feat(Isolation): Support physical isolation, logical isolation`
4. Create Pull Request

If you wish to contribute, please [Pull Request](https://github.com/masastack/MASA.BuildingBlocks/pulls), or send us a [Report Bug](https://github.com/masastack/MASA.BuildingBlocks /issues/new) .



## Contributors

Thanks to all the friends who have contributed to this project.

<a href="https://github.com/masastack/MASA.Contrib/graphs/contributors">
    <img src="https://contrib.rocks/image?repo=masastack/MASA.Contrib" />
</a>



## Code of conduct

This project adopts the Code of Conduct as defined by the Contributors Covenant to clarify the expected behavior of our community. For more information see [MASA Stack Community Code of Conduct](https://github.com/masastack/community/blob/main/CODE-OF-CONDUCT.md).



## ☀️ License Statement

[![MASA.Contrib](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](/LICENSE.txt)

Copyright (c) 2021-present MASA Stack