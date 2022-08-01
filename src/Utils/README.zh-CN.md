中 | [EN](README.md)

# MASA.Utils

MASA.Utils是MASA的工具库，这些工具被[MASA Stack](https://github.com/masastack)和[MASA Labs](https://github.com/masalabs)等项目使用。



## 路线图
* [发行说明](https://github.com/masastack/MASA.Utils/releases)
* [最新路线图](https://github.com/masastack/MASA.Utils/issues/41)



## 特性
* Caching
  * Memory: 内存缓存
  * Redis
  * DistributedMemory: 分布式内存缓存
* Caller
  * [HttpClient](./src/Caller/Masa.Utils.Caller.HttpClient/README.zh-CN.md)
  * [DaprClient](./src/Caller/Masa.Utils.Caller.DaprClient/README.zh-CN.md)
* Data
  * [Elasticsearch](./src/Data/Masa.Utils.Data.Elasticsearch/README.zh-CN.md)
* Development
  * [Development.Dapr](./src/Development/Masa.Utils.Development.Dapr/README.zh-CN.md): Dapr Starter核心库
  * [Development.Dapr.AspNetCore](./src/Development/Masa.Utils.Development.Dapr.AspNetCore/README.zh-CN.md): Dapr AspNetCore库
* Extensions
  * [DependencyInjection](./src/Extensions/Masa.Utils.Extensions.DependencyInjection/README.zh-CN.md): 注入扩展
  * DotNet: DotNet扩展
  * Enums: 枚举扩展
  * Expressions: 表达式扩展
* Ldap
  * Ldap.Novell: 基于Novell的LDAP客户端库
* [Exceptions](https://github.com/masastack/MASA.Utils/blob/main/src/Masa.Utils.Exceptions/README.zh-CN.md): 异常扩展
* Security: 安全
  * Authentication: 身份验证
  * Cryptography: 密码扩展，支持AES加解密、DES加解密、Base64编码、MD5加密、SHA加密等
  * Token: JWT令牌管理



## 如何贡献

1. Fork & Clone
2. Create Feature_xxx branch
3. Commit with commit message, like `feat(Elasticsearch): Support Index management, document management, alias management`
4. Create Pull Request

如果你希望参与贡献，欢迎 [Pull Request](https://github.com/masastack/MASA.Utils/pulls)，或给我们 [报告 Bug](https://github.com/masastack/MASA.Utils/issues/new) 。



## 贡献者

感谢所有为本项目做出过贡献的朋友。

<a href="https://github.com/masastack/MASA.Utils/graphs/contributors">
    <img src="https://contrib.rocks/image?repo=masastack/MASA.Utils" />
</a>



## 行为准则

本项目采用了《贡献者公约》所定义的行为准则，以明确我们社区的预期行为。更多信息请见 [MASA Stack Community Code of Conduct](https://github.com/masastack/community/blob/main/CODE-OF-CONDUCT.md).



## ☀️ 许可声明

[![MASA.Utils](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](/LICENSE.txt)

Copyright (c) 2021-present MASA Stack