中 | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.SequentialGuid

Masa.Contrib.Data.IdGenerator.SequentialGuid是一个有序的id构造器，提供Guid类型的唯一标识

## 用例:

1. 安装`Masa.Contrib.Data.IdGenerator.SequentialGuid`

    ```c#
    Install-Package Masa.Contrib.Data.IdGenerator.SequentialGuid
    ```

2. 使用`Masa.Contrib.Data.IdGenerator.SequentialGuid`

    ``` C#
    builder.Services.AddSequentialGuidGenerator();
    ```

3. 获取Id

    ```
    ISequentialGuidGenerator generator;// 通过DI获取，或者通过IdGeneratorFactory.SequentialGuidGenerator获取
    generator.Create();//创建唯一id
    ```
