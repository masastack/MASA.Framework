[中](README.zh-CN.md) | EN

## Masa.Contrib.Development.DaprStarters.AspNetCore

Responsibilities:

Assist in managing the dapr process to reduce the dependency on docker compose during development

Example：

``` powershell
Install-Package Masa.Contrib.Development.DaprStarters.AspNetCore
```

### Get Started

1. Add DaprStarter to assist in managing the dapr process (recommended to be used in the development environment)

``` C#
builder.Services.AddDaprStarter();
```

### Advanced

1. Specify the configuration in the code

``` C#
builder.Services.AddDaprStarter(opt =>
{
    opt.AppId = "masa-dapr-test";
    opt.AppPort = 5001;
    opt.AppIdSuffix = "";
    opt.DaprHttpPort = 8080;
    opt.DaprGrpcPort = 8081;
});
```

2. The configuration file specifies the configuration

Step 1: Add a `DaprOptions` node to store the DaprStarter startup configuration, modify `Program.cs`

``` appsettings.json
{
  "DaprOptions": {
    "AppId": "masa-dapr-test",
    "AppPort": 5001,
    "AppIdSuffix": "",
    "DaprHttpPort": 8080,
    "DaprGrpcPort": 8081
  }
}
```

Step 2: Register DaprStarter, change `Program.cs`

``` C#
builder.Services.AddDaprStarter(builder.Configuration.GetSection("DaprOptions");
```

Advantages: After the configuration is changed, the dapr process is restarted and updated, and the project does not need to be restarted