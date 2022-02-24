[中](README.zh-CN.md) | EN

## Masa.Contrib.Configuration

Structure:

```c#
IConfiguration
├── Local                                Local node (fixed)
│   ├── Appsettings                      Default local node
│   ├── ├── Platforms                    Custom configuration
│   ├── ├── ├── Name                     Parameter name
├── ConfigurationAPI                     Remote node (fixed)
│   ├── AppId                            Replace-With-Your-AppId
│   ├── AppId ├── Platforms              Custom node
│   ├── AppId ├── Platforms ├── Name     Parameter name
│   ├── AppId ├── DataDictionary         Dictionary (fixed)
```

Example：

```C#
Install-Package Masa.Contrib.Configuration
Install-Package Masa.Contrib.BasicAbility.Dcc //DCC can provide remote configuration capabilities
```json
{
  //Custom configuration
  "Platforms": {
    "Name": "Masa.Demo"
  },
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
  "ConfigObjects": [ "Platforms" ], //The name of the object to be mounted. Here, the Platforms configuration will be mounted under the ConfigurationAPI: <Replace-With-Your-AppId> node
  "Secret": "", //Dcc App key
  "Cluster": "Default"
}
```

Automatically map node relationships：

```c#
public class PlatformOptions : MasaConfigurationOptions
{
    public override SectionTypes SectionType { get; init; } = SectionTypes.Local;

    [JsonIgnore]
    public override string? ParentSection { get; init; } = "Appsettings";

    [JsonIgnore]
    public override string? Section { get; init; } = "Platforms";

    public string Name { get; set; }
}

//Use MasaConfiguration to take over Configuration, and mount the current Configuration to Local:Appsettings section by default
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc(builder.Services);//Use Dcc to extend Configuration capabilities and support remote configuration
});
```

Or manually map node relationships：

```C#
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc(builder.Services);//Use Dcc to extend Configuration capabilities and support remote configuration

    configurationBuilder.UseMasaOptions(options =>
    {
        options.Mapping<PlatformOptions>(SectionTypes.Local, "Appsettings", "Platforms"); //Map the PlatformOptions binding to the Local:Appsettings:Platforms node
    });
});
```

how to use：

```c#
var app = builder.Build();

app.Map("/GetPlatform", ([FromServices] IOptions<PlatformOptions> option) =>
{
    //Recommended (need to automatically or manually map the node relationship before it can be used)
    return System.Text.Json.JsonSerializer.Serialize(option.Value);
});

app.Map("/GetPlatform", ([FromServices] IOptionsMonitor<PlatformOptions> option) =>
{
    //Recommended (need to automatically or manually map the node relationship before it can be used)
    options.OnChange(option =>
    {
        //TODO Configuration update service
    });

    return System.Text.Json.JsonSerializer.Serialize(option.CurrentValue);
});

app.Map("/GetPlatformName", ([FromServices] IConfiguration configuration) =>
{
    //Base
    return configuration["Local:Appsettings:Platforms:Name"];
});

app.Run();
```

How to take over more local nodes？

```c#
builder.AddMasaConfiguration(builder =>{

    builder.AddSection(new ConfigurationBuilder()
                       .SetBasePath(builder.Environment.ContentRootPath)
                       .AddJsonFile("custom.json", true, true), "Custom");//Mount the custom.json configuration under the Local:Custom node
});
```

Tip：

Configuration automatically obtains classes that inherit the IMasaConfigurationOptions interface by default, and maps the node relationship to facilitate obtaining configuration information through IOptions, IOptionsSnapshot, and IOptionsMonitor

The above Platforms is a local configuration, used to demonstrate the effect and usage of the local configuration after being mounted to IConfiguration