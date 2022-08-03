[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.IdGenerator.SimpleGuid

Masa.Contrib.Data.IdGenerator.SimpleGuid is a simple guid constructor that provides a unique identifier of type Guid

## Example:

1. Install `Masa.Contrib.Data.IdGenerator.SimpleGuid`

     ````c#
     Install-Package Masa.Contrib.Data.IdGenerator.SimpleGuid
     ````

2. Use `Masa.Contrib.Data.IdGenerator.SimpleGuid`

     ```` C#
     builder.Services.AddSimpleGuidGenerator();
     ````

3. Get Id

     ````
     IGuidGenerator generator;// Get it through DI, or get it through IdGeneratorFactory.GuidGenerator
     generator.NewId();//Create a unique id
     ````