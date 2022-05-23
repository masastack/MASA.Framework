中 | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.Yitter

Masa.Contrib.Data.IdGenerator.Yitter是基于[Yitter.IdGenerator](https://github.com/yitter/IdGenerator)的分布式id生成器。

## 用例:

1. 安装`Masa.Contrib.Data.IdGenerator.Yitter`

    ```c#
    Install-Package Masa.Contrib.Data.IdGenerator.Yitter
    ```

2. 使用`IdGenerator.Yitter`

    ``` C#
    builder.Services.AddSnowflake();
    ```

3. 获取分布式id

    ```
    IIdGenerator idGenerator;// 通过DI获取
    idGenerator.Generate();//创建分布式id
    ```