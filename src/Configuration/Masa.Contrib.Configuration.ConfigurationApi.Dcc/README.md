[中](README.zh-CN.md) | EN

## Masa.Contrib.Configuration.ConfigurationApi.Dcc

Effect:

Extend the ability of IConfiguration to manage remote configuration through Dcc.

```c#
IConfiguration
├── Local                                Local node (fixed)
├── ConfigurationAPI                     Remote node (fixed Dcc to expand its capacity)
│   ├── AppId                            Replace-With-Your-AppId
│   ├── AppId ├── Redis                  Custom node
│   ├── AppId ├── Redis ├── Host         Parameter
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
  "ConfigObjects": [ "Redis" ], //The name of the object to be mounted, the Redis configuration will be mounted here under the ConfigurationAPI:<Replace-With-Your-AppId> node
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

app.MapGet("/GetRedis", ([FromServices] IOptions<RedisOptions> option) =>
{
    //recommend
    return System.Text.Json.JsonSerializer.Serialize(option.Value);//Or use IOptionsMonitor to support monitoring changes
});

app.MapGet("/GetRedisByMonitor", ([FromServices] IOptionsMonitor<RedisOptions> options) =>
{
    options.OnChange(option =>
    {
        //TODO Configuration update
    });
    return System.Text.Json.JsonSerializer.Serialize(option.CurrentValue);
});

app.MapGet("/GetRedisHost", ([FromServices] IConfiguration configuration) =>
{
    //Obtain the configuration value of the Host of the specified configuration object (ConfigObject) under the specified AppId from the configuration center
    //Format ConfigurationAPI:<Replace-With-Your-AppId>:<Your ConfigObject>:<parameter Host>
    return configuration["ConfigurationAPI:<Replace-With-Your-AppId>:Redis:Host"];
});

app.MapPut("/UpdateRedis", ([FromServices] IConfigurationAPIManage configurationAPIManage,
                               [FromServices] IOptions<CustomDccSectionOptions> configuration,
                               RedisOptions newRedis) =>
{
    //Modify Dcc configuration
    return configurationAPIManage.UpdateAsync(option.Value.Environment,
                                              option.Value.Cluster,
                                              option.Value.AppId,
                                              "<Replace-With-Your-ConfigObject>",newRedis);//Here Replace-With-Your-ConfigObject is Redis
});
app.Run();
```

How to update the configuration:

```c#
var app = builder.Build();

app.MapPut("/UpdateRedis", ([FromServices] IConfigurationAPIManage configurationAPIManage,
                               [FromServices] IOptions<CustomDccSectionOptions> configuration,
                               RedisOptions newRedis) =>
{
    //Modify Dcc configuration
    return configurationAPIManage.UpdateAsync(option.Value.Environment,
                                              option.Value.Cluster,
                                              option.Value.AppId,
                                              "<Replace-With-Your-ConfigObject>"
                                              ,newRedis);
                                              //Here Replace-With-Your-ConfigObject is Redis
});

app.Run();
```

Summarize：

Dcc provides remote configuration management and viewing capabilities for IConfiguration. For the complete capabilities of IConfiguration, please refer to the [document](../../Configuration/Masa.Contrib.Configuration/README.md)

Redis here is remote configuration, which introduces the effect and usage of remote configuration after mounting to IConfiguration. This configuration has nothing to do with Redis in Masa.Contrib.Configuration. It just shows the use of the same configuration information in two sources. Ways and differences in mapping node relationships