[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.Serialization.Yaml

Example:

``` powershelll
Install-Package Masa.Contrib.Data.Serialization.Yaml
```

### Get Started

1. Register `Yaml`, modify `Program.cs`

``` C#
builder.Services.AddYaml();
```

2. Serialization using `IYamlSerializer`

``` C#
var person = new Person
{
    Name = "Abe Lincoln",
    Age = 25,
    HeightInInches = 6f + 4f / 12f,
    Addresses = new Dictionary<string, Address>{
        { "home", new  Address() {
                Street = "2720  Sundown Lane",
                City = "Kentucketsville",
                State = "Calousiyorkida",
                Zip = "99978",
            }},
        { "work", new  Address() {
                Street = "1600 Pennsylvania Avenue NW",
                City = "Washington",
                State = "District of Columbia",
                Zip = "20500",
            }},
    }
};

IYamlSerializer yamlSerializer;// 通过DI获取
var yaml = yamlSerializer.Serialize(person);
```

3. Deserialization using `IYamlDeserializer`

``` C#
var yml = @"
name: George Washington
age: 89
height_in_inches: 5.75
addresses:
  home:
    street: 400 Mockingbird Lane
    city: Louaryland
    state: Hawidaho
    zip: 99970
";
IYamlDeserializer yamlDeserializer; // Get through DI
var people = yamlDeserializer.Deserialize<Person>(yml);
```