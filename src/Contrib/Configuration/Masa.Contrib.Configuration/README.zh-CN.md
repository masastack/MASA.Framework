中 | [EN](README.md)

## Masa.Contrib.Configuration

MasaConfiguration的核心逻辑，并默认提供了本地配置的迁移

结构:

``` c#
IConfiguration
├── Local                           本地节点（固定）
│   ├── Redis                       自定义配置
│   ├── ├── Host                    参数
├── ConfigurationApi                远程节点（固定）
│   ├── AppId                       替换为你的AppId
│   ├── AppId ├── Redis             自定义节点
│   ├── AppId ├── Redis ├── Host    参数
```

用例：

``` C#
Install-Package Masa.Contrib.Configuration
Install-Package Masa.Contrib.Configuration.ConfigurationApi.Dcc // 由Dcc提供远程配置的能力，可根据需要更换其他远程配置的提供者（非必须）
```

### 入门:

1. 修改`appsettings.json`，配置`Dcc`所需参数（远程能力）以及`Redis`配置（用于展示获取本地配置）

``` json
{
  //自定义配置
  "Redis": {
    "Host": "localhost"
  },

  //Dcc配置，用于获取远程配置能力（非必须）
  "DccOptions": {
    "ManageServiceAddress ": "http://localhost:8890",
    "RedisOptions": {
      "Servers": [
        {
          "Host": "localhost",
          "Port": 8889
        }
      ],
      "DefaultDatabase": 0,
      "Password": ""
    }
  }
}
```

2. 注册`MasaConfiguration`，修改`Program.cs`

``` c#
//使用MasaConfiguration接管Configuration，默认会将当前的Configuration挂载到Local节点下
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc();//使用Dcc获取远程配置的能力，由 Masa.Contrib.Configuration.ConfigurationApi.Dcc 提供（非必须）
});
```

3. 新建`RedisOptions`类，配置映射关系

``` c#
/// <summary>
/// 自动映射节点关系
/// </summary>
public class RedisOptions : LocalMasaConfigurationOptions
{
    [JsonIgnore]
    public override string? Section { get; init; } = "Redis";

    public string Host { get; set; }
}
```

> 由于Redis配置在本地配置中，因此类需要继承`LocalMasaConfigurationOptions`

4. 获取配置

``` C#
var app = builder.Build();
app.Map("/GetRedis", ([FromServices] IOptions<RedisOptions> option) =>
{
    //推荐（需要自动或手动映射节点关系后才能使用）
    return System.Text.Json.JsonSerializer.Serialize(option.Value);
});
```

### 进阶

1. 手动指定映射关系，优势：无需更改原来类的继承关系

```C#
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseMasaOptions(options =>
    {
        options.MappingLocal<RedisOptions>("Redis"); //将RedisOptions绑定映射到Local:Redis节点
    });
});
```

2. 如何接管更多的本地节点？

```c#
builder.Services.AddMasaConfiguration(configurationBuilder  =>
{
    configurationBuilder.AddJsonFile("custom.json", true, true);
});//除了默认的ICongiguration，还将custom.json挂载到新的Configuration中
```

3. 如何使用

除了通过IOptions、IOptionsMonitor、IOptionsSnapshot之外，还支持通过`IMasaConfiguration`获取

```c#
IMasaConfiguration masaConfiguration;//从DI获取IMasaConfiguration
masaConfiguration.Local["Redis:Host"];
```

### 提示

Configuration默认自动获取继承LocalMasaConfigurationOptions的类，并映射节点关系，方便通过IOptions、IOptionsSnapshot、IOptionsMonitor获取配置信息

上文Redis为本地配置，用于演示本地配置挂载到IConfiguration后的效果以及使用用法

