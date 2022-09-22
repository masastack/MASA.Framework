中 | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.NormalGuid

Masa.Contrib.Data.IdGenerator.NormalGuid是一个无序的Guid构造器，提供Guid类型的唯一标识

用例:

``` powershell
Install-Package Masa.Contrib.Data.IdGenerator.NormalGuid
```

### 入门

1. 注册Guid构造器，修改`Program.cs`

``` C#
builder.Services.AddSimpleGuidGenerator();
```

2. 获取Id

```
IGuidGenerator generator;// 通过DI获取
generator.NewId();//创建唯一id
```

> 或通过`MasaApp.GetRequiredService<IGuidGenerator>().NewId()`获取