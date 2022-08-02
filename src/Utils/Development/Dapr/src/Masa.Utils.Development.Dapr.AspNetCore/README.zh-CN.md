中 | [EN](README.md)

## Masa.Utils.Development.Dapr.AspNetCore

职责：

协助管理dapr进程，用于开发时减少对docker compose的依赖

### 基本用法：

1. 安装Masa.Utils.Development.Dapr.AspNetCore
```C#
Install-Package Masa.Utils.Development.Dapr.AspNetCore
```

2. 添加DaprStarter协助管理dapr进程（建议在开发环境使用）

```C#
builder.Services.AddDaprStarter();
```

### 高级用法：

1. 代码中指定配置

```C#
builder.Services.AddDaprStarter(opt =>
{
    opt.AppId = "masa-dapr-test";
    opt.AppPort = 5001;
    opt.AppIdSuffix = "";
    opt.DaprHttpPort = 8080;
    opt.DaprGrpcPort = 8081;
});
```

2. 配置文件指定配置

第一步：

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

第二步：

``` C#
builder.Services.AddDaprStarter(builder.Configuration.GetSection("DaprOptions"));
```

优势：支持配置变更后，dapr 进程重启更新，项目无需重新启动
