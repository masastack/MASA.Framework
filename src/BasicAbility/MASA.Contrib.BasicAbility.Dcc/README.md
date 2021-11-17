[中](README.zh-CN.md) | EN

## MASA.Contrib.BasicAbility.Dcc

Effect:

Extend the ability of IConfiguration to manage remote configuration through Dcc.

```c#
IConfiguration
├── Local                                Local node (fixed)
├── ConfigurationAPI                     Remote node (fixed Dcc to expand its capacity)
│   ├── AppId                            Replace-With-Your-AppId
│   ├── AppId ├── Platforms              Custom node
│   ├── AppId ├── Platforms ├── Name     Parameter Name
│   ├── AppId ├── DataDictionary         Dictionary (fixed) The type of Text in DCC is mounted here
```

Example：

```C#
Install-Package MASA.Contrib.Configuration
Install-Package MASA.Contrib.BasicAbility.Dcc //Provides the ability to remotely configure
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
builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc(builder.Services);//Use Dcc

    options.Mapping<CustomDccSectionOptions>(SectionTypes.Local, "Appsettings", ""); //Map CustomDccSectionOptions to the Appsettings node under Local
});

/// <summary>
/// Automatically map node relationships
/// </summary>
public class PlatformOptions : MasaConfigurationOptions
{
    public override SectionTypes SectionType { get; init; } = SectionTypes.ConfigurationAPI;

    [JsonIgnore]
    public virtual string? ParentSection { get; init; } = "AppId";

    [JsonIgnore]
    public virtual string? Section { get; init; } = "Platforms";

    public string Name { get; set; }
}

public class CustomDccSectionOptions
{
    /// <summary>
    /// The environment name.
    /// Get from the environment variable ASPNETCORE_ENVIRONMENT when Environment is null or empty
    /// </summary>
    public string? Environment { get; set; } = null;

    /// <summary>
    /// The cluster name.
    /// </summary>
    public string? Cluster { get; set; }

    /// <summary>
    /// The app id.
    /// </summary>
    public string AppId { get; set; } = default!;

    public List<string> ConfigObjects { get; set; } = default!;

    public string? Secret { get; set; }
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
    //Format ConfigurationAPI:<Replace-With-Your-AppId>:<Your Node Path>:<parameter name>
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

Dcc provides remote configuration management and viewing capabilities for IConfiguration. For the complete capabilities of IConfiguration, please refer to the [document](../../Configuration/MASA.Contrib.Configuration/README.md)

Platforms here is remote configuration, which introduces the effect and usage of remote configuration after mounting to IConfiguration. This configuration has nothing to do with Platforms in MASA.Contrib.Configuration. It just shows the use of the same configuration information in two sources. Ways and differences in mapping node relationships