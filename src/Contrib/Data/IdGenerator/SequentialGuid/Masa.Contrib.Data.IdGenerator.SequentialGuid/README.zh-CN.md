中 | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.SequentialGuid

Masa.Contrib.Data.IdGenerator.SequentialGuid是一个有序的Guid构造器，提供Guid类型的唯一标识

用例:

``` powershell
Install-Package Masa.Contrib.Data.IdGenerator.SequentialGuid
```

### 入门

1. 注册Guid构造器，修改`Program.cs`

``` C#
builder.Services.AddSequentialGuidGenerator();
```

2. 获取Id

```
ISequentialGuidGenerator generator;// 通过DI获取
generator.NewId();//创建唯一id
```

> 或通过`MasaApp.GetRequiredService<ISequentialGuidGenerator>().NewId()`获取

### 配置

* SequentialGuidType：枚举
  * SequentialAsString: MySql、PostgreSql
  * SequentialAsBinary: Oracle
  * SequentialAtEnd: SqlServer (默认)