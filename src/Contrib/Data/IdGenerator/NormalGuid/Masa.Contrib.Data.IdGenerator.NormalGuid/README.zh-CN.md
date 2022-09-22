中 | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.NormalGuid

Masa.Contrib.Data.IdGenerator.NormalGuid是一个简单的Guid构造器，提供Guid类型的唯一标识

用例:

1. 安装`Masa.Contrib.Data.IdGenerator.NormalGuid`

    ```c#
    Install-Package Masa.Contrib.Data.IdGenerator.NormalGuid
    ```

2. 使用`Masa.Contrib.Data.IdGenerator.NormalGuid`

    ``` C#
    builder.Services.AddSimpleGuidGenerator();
    ```

3. 获取Id

    ```
    IGuidGenerator generator;// 通过DI获取，或者通过IdGeneratorFactory.GuidGenerator获取
    generator.NewId();//创建唯一id
    ```
