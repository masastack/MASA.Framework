[中](README.zh-CN.md) | EN

## Masa.Utils.Development.Dapr.AspNetCore

Responsibilities:

Assist in managing the dapr process to reduce the dependency on docker compose during development

### Basic usage:

1. Install Masa.Utils.Development.Dapr.AspNetCore
``` C#
Install-Package Masa.Utils.Development.Dapr.AspNetCore
```

2. Add DaprStarter to assist in managing the dapr process (recommended to be used in the development environment)

``` C#
builder.Services.AddDaprStarter();
```

### Advanced usage:

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

First step:

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

Step 2:

``` C#
builder.Services.AddDaprStarter(builder.Configuration.GetSection("DaprOptions");
```

Advantages: After the configuration is changed, the dapr process is restarted and updated, and the project does not need to be restarted
