中 | [EN](README.md)

## Masa.Contrib.Configuration

结构:

```c#
IConfiguration
├── Local                                本地节点（固定）
│   ├── Platforms                    自定义配置
│   ├── ├── Name                     参数
├── ConfigurationAPI                     远程节点（固定）
│   ├── AppId                            替换为你的AppId
│   ├── AppId ├── Platforms              自定义节点
│   ├── AppId ├── Platforms ├── Name     参数
│   ├── AppId ├── DataDictionary         字典（固定）
```

用例：

```C#
Install-Package Masa.Contrib.Configuration
Install-Package Masa.Contrib.BasicAbility.Dcc //DCC可提供远程配置的能力
```

appsettings.json
```json
{
  //自定义配置
  "Platforms": {
    "Name": "Masa.Demo"
  },
  //Dcc配置，扩展Configuration能力，支持远程配置
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
  },
  "AppId": "Replace-With-Your-AppId",
  "ConfigObjects": [ "Platforms" ], // 要挂载的对象名称,此处会将Platforms配置挂载到ConfigurationAPI:<Replace-With-Your-AppId>节点下
  "Secret": "", //Dcc App 秘钥
  "Cluster": "Default"
}
```

自动映射节点关系：

```c#
/// <summary>
/// 自动映射节点关系
/// </summary>
public class PlatformOptions : LocalMasaConfigurationOptions
{
    [JsonIgnore]
    public override string? Section { get; init; } = "Platforms";

    public string Name { get; set; }
}

//使用MasaConfiguration接管Configuration，默认会将当前的Configuration挂载到Local节点下
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc(builder.Services);//使用Dcc 扩展Configuration能力，支持远程配置
});
```

> 本地配置需要继承LocalMasaConfigurationOptions

或手动添加映射节点关系：

```C#
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc(builder.Services);//使用Dcc 扩展Configuration能力，支持远程配置

    configurationBuilder.UseMasaOptions(options =>
    {
        options.MappingLocal<PlatformOptions>("Platforms"); //将PlatformOptions绑定映射到Local:Platforms节点
    });
});
```

如何使用：

```c#
var app = builder.Build();

app.Map("/GetPlatform", ([FromServices] IOptions<PlatformOptions> option) =>
{
    //推荐（需要自动或手动映射节点关系后才能使用）
    return System.Text.Json.JsonSerializer.Serialize(option.Value);
});

app.Map("/GetPlatform", ([FromServices] IOptionsMonitor<PlatformOptions> option) =>
{
    options.OnChange(option =>
    {
        //TODO 配置更新业务
    });

    return System.Text.Json.JsonSerializer.Serialize(option.CurrentValue);
});//推荐（需要自动或手动映射节点关系后才能使用）

app.Map("/GetPlatformName", ([FromServices] IConfiguration configuration) =>
{
    //基础
    return configuration["Local:Platforms:Name"];
});

app.Run();
```

如何接管更多的本地节点？

```c#
builder.AddMasaConfiguration(builder => builder.AddJsonFile("custom.json", true, true));//除了默认的ICongiguration，还将custom.json挂载到新的Configuration中
```

提示：

Configuration默认自动获取继承LocalMasaConfigurationOptions的类，并映射节点关系，方便通过IOptions、IOptionsSnapshot、IOptionsMonitor获取配置信息

上文Platforms为本地配置，用于演示本地配置挂载到IConfiguration后的效果以及使用用法