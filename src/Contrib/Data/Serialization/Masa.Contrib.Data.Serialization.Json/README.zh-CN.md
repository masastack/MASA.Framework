中 | [EN](README.md)

## Masa.Contrib.Data.Serialization.Json

用例:

``` powershelll
Install-Package Masa.Contrib.Data.Serialization.Json
```

### 入门

1. 注册`Json`, 修改`Program.cs`

``` C#
builder.Services.AddSerialization(builder => builder.UseJson());
```

2. 使用`IJsonSerializer`进行序列化

``` C#
IJsonSerializer jsonSerializer;// 通过DI获取
jsonSerializer.Serialize(new
{
    id = 1,
    name = "序列化"
});
```

3. 使用`IJsonDeserializer`进行反序列化

``` C#
public class UserDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

var json = "{\"id\":1,\"name\":\"反序列化\"}";
IJsonDeserializer jsonDeserializer; // 通过DI获取
var user = jsonDeserializer.Deserialize<UserDto>(json);
```