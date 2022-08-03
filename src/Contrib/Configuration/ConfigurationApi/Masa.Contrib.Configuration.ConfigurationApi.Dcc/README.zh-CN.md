中 | [EN](README.md)

## Masa.Contrib.Configuration.ConfigurationApi.Dcc

作用:

通过Dcc扩展IConfiguration管理远程配置的能力。

```c#
IConfiguration
├── Local                                本地节点（固定）
├── ConfigurationApi                     远程节点（固定 Dcc扩展其能力）
│   ├── AppId                            Replace-With-Your-AppId
│   ├── AppId ├── Redis                  自定义节点
│   ├── AppId ├── Redis ├── Host         参数
```

用例：

```C#
Install-Package Masa.Contrib.Configuration
Install-Package Masa.Contrib.Configuration.ConfigurationApi.Dcc //提供远程配置的能力
```

appsettings.json
```
{
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
  "Environment": "Development",
  "ConfigObjects": [ "Redis" ], //待挂载的对象名, 此处会将Redis配置挂载到ConfigurationApi:<Replace-With-Your-AppId>节点下
  "Secret": "", //Dcc App 秘钥
  "Cluster": "Default"
}

```

```C#
builder.AddMasaConfiguration(configurationBuilder => configurationBuilder.UseDcc());//使用Dcc提供远程配置的能力

/// <summary>
/// 自动映射节点关系
/// </summary>
public class RedisOptions : ConfigurationApiMasaConfigurationOptions
{
    /// <summary>
    /// The app id.
    /// </summary>
    [JsonIgnore]
    public override string AppId { get; set; } = "Replace-With-Your-AppId";

    [JsonIgnore]
    public override string? ObjectName { get; init; } = "Redis";

    public string Host { get; set; }
    
    public int Port { get; set; }
    
    public int DefaultDatabase { get; set; }
}

public class CustomDccSectionOptions : ConfigurationApiMasaConfigurationOptions
{
    /// <summary>
    /// The app id.
    /// </summary>
    [JsonIgnore]
    public override string AppId { get; set; } = "Replace-With-Your-AppId";
    
    /// <summary>
    /// The environment name.
    /// Get from the environment variable ASPNETCORE_ENVIRONMENT when Environment is null or empty
    /// </summary>
    public string? Environment { get; set; } = null;

    /// <summary>
    /// The cluster name.
    /// </summary>
    public string? Cluster { get; set; }

    public List<string> ConfigObjects { get; set; } = default!;

    public string? Secret { get; set; }

    /// <summary>
    /// 将CustomDccSectionOptions挂载到根节点下
    /// </summary>
    [JsonIgnore]
    public virtual string? Section => string.Empty;
}
```

如何使用配置：

```c#
var app = builder.Build();

//在程序入口处读取配置
var redisHost = builder.GetMasaConfiguration().ConfigurationApi.GetDefault().GetValue<string>("Redis:Host");

app.MapGet("/GetRedis", ([FromServices] IOptions<RedisOptions> option) =>
{
    //推荐
    return System.Text.Json.JsonSerializer.Serialize(option.Value);
});

app.MapGet("/GetRedisByMonitor", ([FromServices] IOptionsMonitor<RedisOptions> options) =>
{
    options.OnChange(option =>
    {
        //TODO 配置更新
    });
    return System.Text.Json.JsonSerializer.Serialize(option.CurrentValue);
});

app.MapGet("/GetRedisHost2", ([FromServices] IConfiguration configuration) =>
{
    //从配置中心获取指定AppId下的指定配置对象（ConfigObject）的Host的配置值
    //格式：ConfigurationApi:<自定义AppId>:<自定义的ConfigObject>:<参数名Host>
    return configuration["ConfigurationApi:<Replace-With-Your-AppId>:Redis:Host"];
});

app.Run();
```

如何更新配置


```c#
var app = builder.Build();

app.MapPut("/UpdateRedis", ([FromServices] IConfigurationAPIManage configurationAPIManage,
                               [FromServices] IOptions<CustomDccSectionOptions> configuration,
                               RedisOptions newRedis) =>
{
    //修改Dcc配置
    return configurationAPIManage.UpdateAsync(option.Value.Environment,
                                              option.Value.Cluster,
                                              option.Value.AppId,
                                              "<自定义-ConfigObject>"
                                              ,newRedis);
                                              //此处<自定义-ConfigObject>是Redis
});

app.Run();
```

总结：

Dcc为IConfiguration提供了远程配置的管理以及查看能力，IConfiguration完整的能力请查看[文档](../../Configuration/Masa.Contrib.Configuration/README.zh-CN.md)

此处Redis为远程配置，介绍的是远程配置挂载到IConfiguration之后的效果以及用法，此配置与MASA.Contrib.Configuration中Redis的毫无关系，仅仅是展示同一个配置信息在两个源的使用方式以及映射节点关系的差别