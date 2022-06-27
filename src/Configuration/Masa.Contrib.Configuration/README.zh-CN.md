中 | [EN](README.md)

## Masa.Contrib.Configuration

结构:

```c#
IConfiguration
├── Local                           本地节点（固定）
│   ├── Redis                       自定义配置
│   ├── ├── Host                    参数
├── ConfigurationAPI                远程节点（固定）
│   ├── AppId                       替换为你的AppId
│   ├── AppId ├── Redis             自定义节点
│   ├── AppId ├── Redis ├── Host    参数
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
  "Redis": {
    "Host": "localhost"
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
  "ConfigObjects": [ "Redis" ], // 要挂载的对象名称,此处会将Redis配置挂载到ConfigurationAPI:<Replace-With-Your-AppId>节点下
  "Secret": "", //Dcc App 秘钥
  "Cluster": "Default"
}
```

自动映射节点关系：

```c#
/// <summary>
/// 自动映射节点关系
/// </summary>
public class RedisOptions : LocalMasaConfigurationOptions
{
    [JsonIgnore]
    public override string? Section { get; init; } = "Redis";

    public string Host { get; set; }
}

//使用MasaConfiguration接管Configuration，默认会将当前的Configuration挂载到Local节点下
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc();//使用Dcc 扩展Configuration能力，支持远程配置
});
```

> 本地配置需要继承LocalMasaConfigurationOptions

或手动添加映射节点关系：

```C#
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc();//使用Dcc 扩展Configuration能力，支持远程配置

    configurationBuilder.UseMasaOptions(options =>
    {
        options.MappingLocal<RedisOptions>("Redis"); //将RedisOptions绑定映射到Local:Redis节点
    });
});
```

如何使用：

```c#
var app = builder.Build();

app.Map("/GetRedis", ([FromServices] IOptions<RedisOptions> option) =>
{
    //推荐（需要自动或手动映射节点关系后才能使用）
    return System.Text.Json.JsonSerializer.Serialize(option.Value);
});

app.Map("/GetRedis", ([FromServices] IOptionsMonitor<RedisOptions> option) =>
{
    options.OnChange(option =>
    {
        //TODO 配置更新业务
    });

    return System.Text.Json.JsonSerializer.Serialize(option.CurrentValue);
});//推荐（需要自动或手动映射节点关系后才能使用）

app.Map("/GetRedisformHost", ([FromServices] IConfiguration configuration) =>
{
    //基础
    return configuration["Local:Redis:Host"];
});

app.Run();
```

如何接管更多的本地节点？

```c#
builder.AddMasaConfiguration(builder => builder.AddJsonFile("custom.json", true, true));//除了默认的ICongiguration，还将custom.json挂载到新的Configuration中
```

提示：

Configuration默认自动获取继承LocalMasaConfigurationOptions的类，并映射节点关系，方便通过IOptions、IOptionsSnapshot、IOptionsMonitor获取配置信息

上文Redis为本地配置，用于演示本地配置挂载到IConfiguration后的效果以及使用用法

