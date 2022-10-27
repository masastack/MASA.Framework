中 | [EN](README.md)

## Masa.Contrib.Development.DaprStarters.AspNetCore

职责：

协助管理dapr进程，用于开发时减少对docker compose的依赖

用例：

``` powershell
Install-Package Masa.Contrib.Development.DaprStarters.AspNetCore
```

### 入门

1添加DaprStarter协助管理dapr进程（建议在开发环境使用）

```C#
builder.Services.AddDaprStarter();
```

### 进阶

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

第一步：新增`DaprOptions`节点，用于存放DaprStarter启动配置，修改`Program.cs`

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

第二步：注册DaprStarter，更改`Program.cs`

``` C#
builder.Services.AddDaprStarter();
```

优势：支持配置变更后，dapr 进程重启更新，项目无需重新启动
