[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.Serialization.Json

Example:

``` powershelll
Install-Package Masa.Contrib.Data.Serialization.Json
```

### Get Started

1. Register `Json`, modify `Program.cs`

``` C#
builder.Services.AddSerialization(builder => builder.UseJson());
```

2. Serialization using `IJsonSerializer`

``` C#
IJsonSerializer jsonSerializer;// Get through DI
jsonSerializer.Serialize(new
{
     id = 1,
     name = "serialization"
});
```

3. Deserialization using `IJsonDeserializer`

``` C#
public class UserDto
{
     [JsonPropertyName("id")]
     public int Id { get; set; }

     [JsonPropertyName("name")]
     public string Name { get; set; }
}

var json = "{\"id\":1,\"name\":\"deserialize\"}";
IJsonDeserializer jsonDeserializer; // Get through DI
var user = jsonDeserializer.Deserialize<UserDto>(json);
```