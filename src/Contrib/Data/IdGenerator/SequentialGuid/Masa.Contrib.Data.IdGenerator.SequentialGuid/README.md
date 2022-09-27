[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.IdGenerator.SequentialGuid

Masa.Contrib.Data.IdGenerator.SequentialGuid is an ordered Guid constructor that provides a unique identifier of the Guid type

Example:

``` powershell
Install-Package Masa.Contrib.Data.IdGenerator.SequentialGuid
```

### Get Started

1. Register the Guid constructor and modify `Program.cs`

``` C#
builder.Services.AddSequentialGuidGenerator();
```

2. Get Id

```
ISequentialGuidGenerator generator;// Get through DI
generator.NewId();//Create a unique id
```

> Or Use `MasaApp.GetRequiredService<ISequentialGuidGenerator>().NewId()`

### Configure

* SequentialGuidType: enumeration
    * SequentialAsString: MySql, PostgreSql
    * SequentialAsBinary: Oracle
    * SequentialAtEnd: SqlServer (default)