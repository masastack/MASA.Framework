Medium | [EN](README.md)

## Masa.Contrib.Data.IdGenerator.SimpleGuid

Masa.Contrib.Data.IdGenerator.SimpleGuid is a simple id constructor that provides a unique identifier of type Guid

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
     generator.Create();//Create a unique id
     ````