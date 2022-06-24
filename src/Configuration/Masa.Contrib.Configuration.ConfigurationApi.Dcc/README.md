[中](README.zh-CN.md) | EN

## Masa.Contrib.Configuration.ConfigurationApi.Dcc

Effect:

Extend the ability of IConfiguration to manage remote configuration through Dcc.

```c#
IConfiguration
├── Local                                Local node (fixed)
├── ConfigurationAPI                     Remote node (fixed Dcc to expand its capacity)
│   ├── AppId                            Replace-With-Your-AppId
│   ├── AppId ├── Platforms              Custom node
│   ├── AppId ├── Platforms ├── Name     Parameter Name
```

Example：

```C#
Install-Package Masa.Contrib.Configuration
Install-Package Masa.Contrib.Configuration.ConfigurationApi.Dcc //Provides the ability to remotely configure
```

appsettings.json
```
{
  //Dcc configuration, extended Configuration capabilities, support remote configuration
  "DccOptions": {
    "ManageServiceAddress": "http://localhost:8890",
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
  "ConfigObjects": [ "Platforms" ], //The name of the object to be mounted, the Platforms configuration will be mounted here under the ConfigurationAPI:<Replace-With-Your-AppId> node
  "Secret": "", //Dcc App key
  "Cluster": "Default"
}

```

```C#
builder.AddMasaConfiguration(configurationBuilder => configurationBuilder.UseDcc());//Ability to provide remote configuration using Dcc

/// <summary>
/// Automatically map node relationships
/// </summary>
public class RedisOptions : ConfigurationApiMasaConfigurationOptions
{
    /// <summary>
    /// The app id.
    /// </summary>
    [JsonIgnore]
    public override string AppId { get; set; } = "Replace-With-Your-AppId";

    /// <summary>
    /// 配置对象名称
    /// </summary>
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
    /// Mount CustomDccSectionOptions under the root node
    /// </summary>
    [JsonIgnore]
    public virtual string? Section => string.Empty;
}
```

How to use configuration：

```c#
var app = builder.Build();

app.MapGet("/GetPlatform", ([FromServices] IOptions<PlatformOptions> option) =>
{
    //recommend
    return System.Text.Json.JsonSerializer.Serialize(option.Value);//Or use IOptionsMonitor to support monitoring changes
});

app.MapGet("/GetPlatformByMonitor", ([FromServices] IOptionsMonitor<PlatformOptions> options) =>
{
    options.OnChange(option =>
    {
        //TODO Configuration update
    });
    return System.Text.Json.JsonSerializer.Serialize(option.CurrentValue);
});

app.MapGet("/GetPlatformName", ([FromServices] IConfiguration configuration) =>
{
    //Obtain the configuration value of the Name of the specified configuration object (ConfigObject) under the specified AppId from the configuration center
    //Format ConfigurationAPI:<Replace-With-Your-AppId>:<Your ConfigObject>:<parameter name>
    return configuration["ConfigurationAPI:<Replace-With-Your-AppId>:Platforms:Name"];
});

app.MapPut("/UpdatePlatform", ([FromServices] IConfigurationAPIManage configurationAPIManage,
                               [FromServices] IOptions<CustomDccSectionOptions> configuration,
                               PlatformOptions newPlatform) =>
{
    //Modify Dcc configuration
    return configurationAPIManage.UpdateAsync(option.Value.Environment,
                                              option.Value.Cluster,
                                              option.Value.AppId,
                                              "<Replace-With-Your-ConfigObject>",newPlatform);//Here Replace-With-Your-ConfigObject is Platforms
});
app.Run();
```

How to update the configuration:

```c#
var app = builder.Build();

app.MapPut("/UpdatePlatform", ([FromServices] IConfigurationAPIManage configurationAPIManage,
                               [FromServices] IOptions<CustomDccSectionOptions> configuration,
                               PlatformOptions newPlatform) =>
{
    //Modify Dcc configuration
    return configurationAPIManage.UpdateAsync(option.Value.Environment,
                                              option.Value.Cluster,
                                              option.Value.AppId,
                                              "<Replace-With-Your-ConfigObject>"
                                              ,newPlatform);
                                              //Here Replace-With-Your-ConfigObject is Platforms
});

app.Run();
```

Summarize：

Dcc provides remote configuration management and viewing capabilities for IConfiguration. For the complete capabilities of IConfiguration, please refer to the [document](../../Configuration/Masa.Contrib.Configuration/README.md)

Platforms here is remote configuration, which introduces the effect and usage of remote configuration after mounting to IConfiguration. This configuration has nothing to do with Platforms in Masa.Contrib.Configuration. It just shows the use of the same configuration information in two sources. Ways and differences in mapping node relationships