[中](README.zh-CN.md) | EN

## Masa.Contrib.Configuration

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

```C#
Install-Package Masa.Contrib.Configuration
Install-Package Masa.Contrib.BasicAbility.Dcc //DCC can provide remote configuration capabilities
​```json
{
  //Custom configuration
  "Redis": {
    "Host": "localhost"
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
  "ConfigObjects": [ "Redis" ], //The name of the object to be mounted. Here, the Redis configuration will be mounted under the ConfigurationApi: <Replace-With-Your-AppId> node
  "Secret": "", //Dcc App key
  "Cluster": "Default"
}
```

Automatically map node relationships：

```c#
public class RedisOptions : LocalMasaConfigurationOptions
{
    [JsonIgnore]
    public override string? Section { get; init; } = "Redis";

    public string Host { get; set; }
}

//Use MasaConfiguration to take over Configuration, and mount the current Configuration to Local section by default
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc(builder.Services);//Use Dcc to extend Configuration capabilities and support remote configuration
});
```

> Local configuration needs to inherit LocalMasaConfigurationOptions

Or manually map node relationships：

```C#
builder.AddMasaConfiguration(configurationBuilder =>
{
    //configurationBuilder.UseDcc(builder.Services);//Use Dcc to extend Configuration capabilities and support remote configuration

    configurationBuilder.UseMasaOptions(options =>
    {
        options.MappingLocal<RedisOptions>("Redis"); //Map the RedisOptions binding to the Local:Redis node
    });
});
```

how to use：

```c#
var app = builder.Build();

app.Map("/GetRedis", ([FromServices] IOptions<RedisOptions> option) =>
{
    //Recommended (need to automatically or manually map the node relationship before it can be used)
    return System.Text.Json.JsonSerializer.Serialize(option.Value);
});

app.Map("/GetRedis", ([FromServices] IOptionsMonitor<RedisOptions> option) =>
{
    //Recommended (need to automatically or manually map the node relationship before it can be used)
    options.OnChange(option =>
    {
        //TODO Configuration update service
    });

    return System.Text.Json.JsonSerializer.Serialize(option.CurrentValue);
});

app.Map("/GetRedisHost", ([FromServices] IConfiguration configuration) =>
{
    //Base
    return configuration["Local:Redis:Host"];
});

app.Run();
```

How to take over more local nodes？

```c#
builder.AddMasaConfiguration(builder => builder.AddJsonFile("custom.json", true, true));//In addition to the default ICongiguration, mount custom.json into the new Configuration
```

Tip：

Configuration automatically obtains classes that inherit LocalMasaConfigurationOptions by default, and maps node relationships to facilitate obtaining configuration information through IOptions, IOptionsSnapshot, and IOptionsMonitor

The above Redis is a local configuration, which is used to demonstrate the effect and usage of the local configuration after it is mounted to IConfiguration