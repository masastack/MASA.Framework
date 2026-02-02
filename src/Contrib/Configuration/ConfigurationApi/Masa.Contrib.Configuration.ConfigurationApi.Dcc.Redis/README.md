[中](README.zh-CN.md) | EN

## Masa.Contrib.Configuration.ConfigurationApi.Dcc

Provider of remote capabilities of MasaConfiguration (Dcc needs to be configured)

Extends IConfiguration's ability to manage remote configuration through Dcc.

``` structure
IConfiguration
├── Local                                Local node (fixed)
├── ConfigurationApi                     Remote node (fixed Dcc to expand its capacity)
│   ├── AppId                            Replace-With-Your-AppId
│   ├── AppId ├── Redis                  Custom node
│   ├── AppId ├── Redis ├── Host         Parameter
```

Example：

``` powershell
Install-Package Masa.Contrib.Configuration //The core of MasaConfiguration
Install-Package Masa.Contrib.Configuration.ConfigurationApi.Dcc //Provides the ability to remotely configure
```

### getting Started:

1. Modify `appsettings.json` and configure the parameters required by `Dcc` (remote capability)

``` C#
{
  //Dcc configuration, expand Configuration capabilities, support remote configuration
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
  }
}
```

2. Register `MasaConfiguration`, and use `Dcc`, modify `Program.cs`

``` C#
builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc()
});//Use Dcc to provide remote configuration capabilities
```

3. Create a new `RedisOptions` class and configure the mapping relationship

```
/// <summary>
/// Automatically map node relationships
/// </summary>
public class RedisOptions : ConfigurationApiMasaConfigurationOptions
{
    /// <summary>
    /// Replace with the AppId of the Redis configuration on the Dcc platform
    /// </summary>
    [JsonIgnore]
    public override string AppId { get; set; } = "Replace-With-Your-AppId";

    /// <summary>
    /// Replace with the name of the configuration object to which the Redis configuration on the Dcc platform belongs, which is the same as the class name when not rewritten
    /// </summary>
    [JsonIgnore]
    public override string? ObjectName { get; init; } = "Redis";

    public string Host { get; set; }

    public int Port { get; set; }

    public int DefaultDatabase { get; set; }
}
```

> Since Redis is configured in the Dcc platform, the class needs to inherit `ConfigurationApiMasaConfigurationOptions`

4. Get configuration

``` C#
var app = builder.Build();

app.Map("/GetRedis", ([FromServices] IOptions<RedisOptions> option) =>
{
    //recommended (requires automatic or manual mapping of node relationships before it can be used)
    return System.Text.Json.JsonSerializer.Serialize(option.Value);
});
```

### Advanced

1. Manually specify the mapping relationship, advantage: no need to change the inheritance relationship of the original class

``` C#
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
    configurationBuilder.UseMasaOptions(options =>
    {
        options.MappingConfigurationApi<RedisOptions>("Replace-With-Your-AppId", "Redis"); //Map RedisOptions binding to ConfigurationApi:AppId:Redis node
    });
});
```

2. How to use

In addition to IOptions, IOptionsMonitor, and IOptionsSnapshot, it also supports getting through `IMasaConfiguration`

``` c#
IMasaConfiguration masaConfiguration;//Get IMasaConfiguration from DI
masaConfiguration.ConfigurationApi["<Replace-With-Your-AppId>:Redis:Host"];
```

### Summarize

Dcc provides remote configuration management and viewing capabilities for IConfiguration. For the complete capabilities of IConfiguration, please refer to [document](../../Configuration/Masa.Contrib.Configuration/README.zh-CN.md)

Here Redis is a remote configuration, which introduces the effect and usage of the remote configuration after it is mounted to IConfiguration. This configuration has nothing to do with Redis in MASA.Contrib.Configuration, but only shows the use of the same configuration information in two sources. Differences in methods and mapping node relationships