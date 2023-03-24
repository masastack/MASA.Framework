中 | [EN](README.md)

## Masa.Contrib.Data.Serialization.Yaml

用例:

``` powershelll
Install-Package Masa.Contrib.Data.Serialization.Yaml
```

### 入门

1. 注册`Yaml`, 修改`Program.cs`

``` C#
builder.Services.AddSerialization(builder => builder.UseYaml());
```

2. 使用`IYamlSerializer`进行序列化

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

3. 使用`IYamlDeserializer`进行反序列化

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
IYamlDeserializer yamlDeserializer; // 通过DI获取
var people = yamlDeserializer.Deserialize<Person>(yml);
```