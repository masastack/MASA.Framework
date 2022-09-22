[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.IdGenerator.SimpleGuid

Masa.Contrib.Data.IdGenerator.NormalGuid is an unordered Guid constructor that provides a unique identifier for the Guid type

Example:

``` powershell
Install-Package Masa.Contrib.Data.IdGenerator.NormalGuid
```

### Get Started

1. Register the Guid constructor and modify `Program.cs`

``` C#
builder.Services.AddSimpleGuidGenerator();
```

2. Get Id

```
IGuidGenerator generator;// Get through DI
generator.NewId();//Create a unique id
```

> or Use `MasaApp.GetRequiredService<IGuidGenerator>().NewId()`