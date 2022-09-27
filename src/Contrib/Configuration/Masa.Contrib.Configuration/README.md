[中](README.zh-CN.md) | EN

## Masa.Contrib.Configuration

The core logic of MasaConfiguration, and provides local configuration migration by default

Structure:

```c#
IConfiguration
├── Local                                Local node (fixed)
│   ├── Redis                            Custom configuration
│   ├── ├── Host                         Parameter
├── ConfigurationApi                     Remote node (fixed)
│   ├── AppId                            Replace-With-Your-AppId
│   ├── AppId ├── Redis                  Custom node
│   ├── AppId ├── Redis ├── Host         Parameter
```

Example：


``` C#
Install-Package Masa.Contrib.Configuration
Install-Package Masa.Contrib.Configuration.ConfigurationApi.Dcc // The ability of remote configuration is provided by Dcc, and other remote configuration providers can be replaced as needed (not required)
```

### getting Started:

1. Modify `appsettings.json`, configure the parameters required by `Dcc` (remote capability) and `Redis` configuration (used to demonstrate obtaining local configuration)

``` json
{
  //Custom configuration
  "Redis": {
    "Host": "localhost"
  },

  //Dcc configuration, used to obtain remote configuration capabilities (not required)
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

2. Register `MasaConfiguration`, modify `Program.cs`

``` c#
//Use MasaConfiguration to take over the Configuration, by default the current Configuration will be mounted under the Local node
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc();//The ability to use Dcc to obtain remote configuration, provided by Masa.Contrib.Configuration.ConfigurationApi.Dcc (not required)
});
```

3. Create a new `RedisOptions` class and configure the mapping relationship

``` c#
/// <summary>
/// Automatically map node relationships
/// </summary>
public class RedisOptions : LocalMasaConfigurationOptions
{
    [JsonIgnore]
    public override string? Section { get; init; } = "Redis";

    public string Host { get; set; }
}
```

> Since Redis is configured in local configuration, the class needs to inherit `LocalMasaConfigurationOptions`

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
    configurationBuilder.UseMasaOptions(options =>
    {
        options.MappingLocal<RedisOptions>("Redis"); //Map RedisOptions binding to Local:Redis node
    });
});
```

2. How to take over more local nodes?

``` c#
builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.AddJsonFile("custom.json", true, true);
});//In addition to the default ICongiguration, also mount custom.json into the new Configuration
```

3. How to use

In addition to IOptions, IOptionsMonitor, and IOptionsSnapshot, it also supports getting through `IMasaConfiguration`

``` c#
IMasaConfiguration masaConfiguration;//Get IMasaConfiguration from DI
masaConfiguration.Local["Redis:Host"];
```

### Tip

Configuration automatically obtains classes that inherit LocalMasaConfigurationOptions by default, and maps node relationships to facilitate obtaining configuration information through IOptions, IOptionsSnapshot, and IOptionsMonitor

The above Redis is a local configuration, which is used to demonstrate the effect and usage of the local configuration after it is mounted to IConfiguration