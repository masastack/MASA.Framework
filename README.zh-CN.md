﻿中 | [EN](README.md)

[![codecov](https://codecov.io/gh/masastack/MASA.Framework/branch/main/graph/badge.svg?token=87TPNHUHW2)](https://codecov.io/gh/masastack/MASA.Framework)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=masastack_MASA.Framework&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=masastack_MASA.Framework)



# MASA.Framework

.NET下一代微服务开发框架，提供基于分布式应用运行时--Dapr云原生最佳实践，能够快速实现分布式、微服务、DDD，SaaS等现代应用开发

![Framework](https://s2.loli.net/2022/08/08/ZVT7De239abvYnw.png)



## 文档

[https://docs.masastack.com/Framework/guide/concepts.html](https://docs.masastack.com/Framework/guide/concepts.html)



## 路线图

* [发行说明](https://github.com/masastack/MASA.Framework/releases)
* [最新路线图](https://github.com/masastack/MASA.Framework/issues/101)



## 特性

以下是Framework提供的构件块能力：

![feature.png](https://s2.loli.net/2022/08/08/ELBPiYvSj6KwNg8.png)



## 快速使用

### 必要条件

* 安装[.NET SDK 6.0](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0)

#### 安装模板

``` shell
dotnet new --install Masa.Template
```

#### 创建项目

``` shell
dotnet new masafx -o Masa.Framework.Demo
```

> 或通过Visual Studio选择`MASA Framework Project`模板进行创建

#### 启动项目

``` shell
dotnet run
```



## 如何贡献

1. Fork & Clone
2. Create Feature_xxx branch
3. Commit with commit message, like `feat(Isolation): Support physical isolation, logical isolation`
4. Create Pull Request

如果你希望参与贡献，欢迎 [Pull Request](https://github.com/masastack/MASA.Framework/pulls)，或给我们 [报告 Bug](https://github.com/masastack/MASA.Framework/issues/new) 。



## 贡献者

感谢所有为本项目做出过贡献的朋友。

<a href="https://github.com/masastack/MASA.Framework/graphs/contributors">
    <img src="https://contrib.rocks/image?repo=masastack/MASA.Framework" />
</a>



## 行为准则

本项目采用了《贡献者公约》所定义的行为准则，以明确我们社区的预期行为。更多信息请见 [MASA Stack Community Code of Conduct](https://github.com/masastack/community/blob/main/CODE-OF-CONDUCT.md).



## ☀️ 许可声明

[![MASA.Framework](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](/LICENSE.txt)

Copyright (c) 2021-present MASA Stack