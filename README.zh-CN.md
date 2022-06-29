中 | [EN](README.md)

[![codecov](https://codecov.io/gh/masastack/MASA.Contrib/branch/main/graph/badge.svg?token=87TPNHUHW2)](https://codecov.io/gh/masastack/MASA.Contrib)

# MASA.Contrib

MASA.Contrib是基于[MASA.BuildingBlocks](https://github.com/masastack/MASA.BuildingBlocks)提供开放, 社区驱动的可重用组件，用于构建网格应用程序。这些组件将被[MASA Stack](https://github.com/masastack)和[MASA Labs](https://github.com/masalabs)等项目使用。



## 路线图
* [发行说明](https://github.com/masastack/MASA.Contrib/releases)
* [最新路线图](https://github.com/masastack/MASA.Contrib/issues/42)



## 特性
* AutoComplete: 使搜索更简单
  * [ElasticSearch](./src/SearchEngine/Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch/README.zh-CN.md)
* Configuration: 配置中心
  * [Configuration核心、提供本地配置](./src/Configuration/Masa.Contrib.Configuration/README.zh-CN.md)
  * ConfigurationAPI
    * [Dcc](./src/Configuration/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.zh-CN.md)
* [CQRS](./src/ReadWriteSpliting/Cqrs/Masa.Contrib.ReadWriteSpliting.Cqrs/README.zh-CN.md)
* Data:
  * [EntityFrameworkCore](./src/Data/Masa.Contrib.Data.EntityFrameworkCore/README.zh-CN.md)
    * [SqlServer](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.SqlServer/README.zh-CN.md)
    * [Pomelo.MySql](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Pomelo.MySql/README.zh-CN.md): 推荐
    * [MySql](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.MySql/README.zh-CN.md)
    * [Sqlite](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Sqlite/README.zh-CN.md)
    * [Cosmos](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Cosmos/README.zh-CN.md)
    * [InMemory](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.InMemory/README.zh-CN.md)
    * [Oracle](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.Oracle/README.zh-CN.md)
    * [PostgreSql](./src/Data/Masa.Contrib.Data.EntityFrameworkCore.PostgreSql/README.zh-CN.md)
  * [Data.Contracts.EF](./src/Data/Masa.Contrib.Data.Contracts.EF/): 数据规约
  * UoW: 工作单元
    * [EFCore](./src/Data/Masa.Contrib.Data.UoW.EF/README.zh-CN.md)
  * IdGenerator: 唯一ID生成器
    * [NormalGuid](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.NormalGuid/README.zh-CN.md): 普通的Guid
    * [SequentialGuid](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.SequentialGuid/README.zh-CN.md): 有序的Guid
    * [Snowflake](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.Snowflake/README.zh-CN.md): Single version snowflake id
    * [Snowflake.Distributed.Redis](./src/Data/IdGenerator/Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis/README.zh-CN.md): 分布式雪花id
  * Mapping: 对象映射
    * [Mapster](./src/Data/Mapping/Masa.Contrib.Data.Mapping.Mapster/README.zh-CN.md)
* [DDD](./src/Ddd/Masa.Contrib.Ddd.Domain/README.zh-CN.md)
  * [Ddd.Domain.Repository.EF](./src/Ddd/Masa.Contrib.Ddd.Domain.Repository.EF/README.zh-CN.md): 提供仓储服务
* Dispatcher
  * [EventBus](./src/Dispatcher/Masa.Contrib.Dispatcher.Events/README.zh-CN.md): 进程内事件
  * [IntegrationEventBus](./src/Dispatcher/Masa.Contrib.Dispatcher.IntegrationEvents.Dapr/README.zh-CN.md): 跨进程事件
    * [IntegrationEvents.EventLogs.EF](./src/Dispatcher/Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF/README.zh-CN.md): 为跨进程事件提供消息管理服务
* Isolation: 支持物理隔离、逻辑隔离
  * [UoW.EF](./src/Isolation/Masa.Contrib.Isolation.UoW.EF/README.zh-CN.md)
  * [MultiEnvironment](./src/Isolation/Masa.Contrib.Isolation.MultiEnvironment/README.zh-CN.md): 多环境
  * [MultiTenant](./src/Isolation/Masa.Contrib.Isolation.MultiTenant/README.zh-CN.md): 多租户
* [MinimalAPI](./src/Service/Masa.Contrib.Service.MinimalAPIs/README.zh-CN.md): 支持类似Controller的API分类聚合
* Storage: 云存储
  * [阿里云存储](./src/Storage/Masa.Contrib.Storage.ObjectStorage.Aliyun/README.zh-CN.md)
* 业务能力
  * [Dcc](./src/BasicAbility/Masa.Contrib.BasicAbility.Dcc/README.zh-CN.md): 分布式配置中心
  * [PM](./src/BasicAbility/Masa.Contrib.BasicAbility.Pm/README.zh-CN.md): 项目管理
  * [Scheduler](./src/BasicAbility/Masa.Contrib.BasicAbility.Scheduler/README.zh-CN.md): 分布式调度中心
  * [TSC](./src/BasicAbility/Masa.Contrib.BasicAbility.Tsc/README.zh-CN.md): 故障排除平台


## 如何克隆
```
git clone --recursive https://github.com/masastack/MASA.Contrib.git
```



## 如何贡献

1. Fork & Clone
2. Create Feature_xxx branch
3. Commit with commit message, like `feat(Isolation): Support physical isolation, logical isolation`
4. Create Pull Request

如果你希望参与贡献，欢迎 [Pull Request](https://github.com/masastack/MASA.BuildingBlocks/pulls)，或给我们 [报告 Bug](https://github.com/masastack/MASA.BuildingBlocks/issues/new) 。



## 贡献者

感谢所有为本项目做出过贡献的朋友。

<a href="https://github.com/masastack/MASA.Contrib/graphs/contributors">
    <img src="https://contrib.rocks/image?repo=masastack/MASA.Contrib" />
</a>



## 行为准则

本项目采用了《贡献者公约》所定义的行为准则，以明确我们社区的预期行为。更多信息请见 [MASA Stack Community Code of Conduct](https://github.com/masastack/community/blob/main/CODE-OF-CONDUCT.md).



## ☀️ 许可声明

[![MASA.Contrib](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](/LICENSE.txt)

Copyright (c) 2021-present MASA Stack