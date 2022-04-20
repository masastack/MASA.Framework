[中](README.zh-CN.md) | EN

[![codecov](https://codecov.io/gh/masastack/MASA.Contrib/branch/main/graph/badge.svg?token=87TPNHUHW2)](https://codecov.io/gh/masastack/MASA.Contrib)

# MASA.Contrib

The purpose of MASA.Contrib is based on [MASA.BuildingBlocks](https://github.com/masastack/MASA.BuildingBlocks) to provide open, community driven reusable components for building mesh applications.  These components will be used by the [MASA Stack](https://github.com/masastack) and [MASA Labs](https://github.com/masalabs) projects.



## Roadmap
* [Release Notes](https://github.com/masastack/MASA.Contrib/releases)
* [Latest Roadmap](https://github.com/masastack/MASA.Contrib/issues/40)



## Features
* AutoComplete: make searching easier
  * [ElasticSearch](./src/SearchEngine/Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch/README.md)
* Configuration: Configuration Center
  * [Configuration core, provide local configuration](./src/Configuration/Masa.Contrib.Configuration/README.md)
  * ConfigurationAPI
    * [Dcc](./src/Configuration/Masa.Contrib.Configuration/README.md)
* [CQRS](./src/ReadWriteSpliting/Cqrs/Masa.Contrib.ReadWriteSpliting.Cqrs/README.md)
* [DDD](./src/Ddd/Masa.Contrib.Ddd.Domain/README.md)
* Dispatcher
  * [EventBus](./src/Dispatcher/Masa.Contrib.Dispatcher.Events/README.md): In-process events
  * [IntegrationEventBus](./src/Dispatcher/Masa.Contrib.Dispatcher.IntegrationEvents.Dapr/README.md): Cross-process events
* Isolation: Support physical isolation, logical isolation
  * [UoW.EF](./src/Isolation/Masa.Contrib.Isolation.UoW.EF/README.md)
  * [MultiEnvironment](./src/Isolation/Masa.Contrib.Isolation.MultiEnvironment/README.md): MultiEnvironment
  * [MultiTenant](./src/Isolation/Masa.Contrib.Isolation.MultiTenant/README.md): Multi-tenancy
* [MinimalAPI](./src/Service/Masa.Contrib.Service.MinimalAPIs/README.md): Support API classification aggregation similar to Controller
* UoW: unit of work
  * [EFCore](./src/Data/Masa.Contrib.Data.UoW.EF/README.md)
* Storage: cloud storage
  * [Aliyun Storage](./src/Storage/Masa.Contrib.Storage.ObjectStorage.Aliyun/README.md)
* Operational capacity
  * Project management
    * PM



## How to download
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