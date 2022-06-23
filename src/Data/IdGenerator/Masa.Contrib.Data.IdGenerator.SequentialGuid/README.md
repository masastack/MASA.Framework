[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.IdGenerator.SequentialGuid

Masa.Contrib.Data.IdGenerator.SequentialGuid is an ordered Guid constructor that provides a unique identifier of the Guid type

## Example:

1. Install `Masa.Contrib.Data.IdGenerator.SequentialGuid`

     ````c#
     Install-Package Masa.Contrib.Data.IdGenerator.SequentialGuid
     ````

2. Use `Masa.Contrib.Data.IdGenerator.SequentialGuid`

     ```` C#
     builder.Services.AddSequentialGuidGenerator();
     ````

3. Get Id

     ````
     ISequentialGuidGenerator generator;// Obtained through DI, or obtained through IdGeneratorFactory.SequentialGuidGenerator
     generator.Create();//Create a unique id
     ````