中 | [EN](README.md)

## Masa.Utils.Development.Dapr

Dapr Starter核心库

职责：

为Masa.Utils.Development.Dapr.AspNetCore 提供核心支撑，支持windows、linux、OSX

dapr进程的启动、停止、刷新、dapr守护进程均由此类库提供

### 用法：

1. 安装Masa.Utils.Development.Dapr.AspNetCore
```C#
Install-Package Masa.Utils.Development.Dapr.AspNetCore
```

2. 添加DaprStarter协助管理dapr进程（建议在开发环境使用）

```C#
builder.Services.AddDaprStarterCore();
```

3. 根据需要在指定位置注入IDaprProcess, 之后调用Start方法即可启动dapr进程或者交由Masa.Utils.Development.Dapr.AspNetCore管理dapr进程，相关文档可[查看](../Masa.Utils.Development.Dapr.AspNetCore/README.zh-CN.md)

例如：

新建DaprController

``` C# DaprController.cs
public class DaprController : ControllerBase
{
    private readonly IDaprProcess _daprProcess;

    private readonly DaprOptions _options;

    public DaprController(IDaprProcess daprProcess, IOptions<DaprOptions> options)
    {
        _daprProcess = daprProcess;
        _options = options.Value;
    }

    [HttpGet(Name = "Start")]
    public string Start()
    {
        _daprProcess.Start(_options);
        return "start success";
    }

    [HttpGet(Name = "Stop")]
    public string Stop()
    {
        _daprProcess.Stop();
        return "stop success";
    }
}
```

## 注意

1. 库中有使用到netstat命令，请确保netstat命令是可用的

> Windows系统默认支持netstat命令无需特殊安装
>
> Linux与OSX需要自行确认确认电脑是否安装netstat

打开终端输入以下命令确认电脑支持netstat命令：

```
netstat -h
```

例：ubuntu：

```
apt-get install net-tools
```

2. AppId、AppIdSuffix强烈建议不要输入含.的字符串，否则会导致dapr调用出现问题，推荐使用-
   1. Dapr AppID遵循FQDN格式，其中包括目标命名空间
   2. FQDN是通过符号.来拼接域名的

### 规则

dapr AppId命名规则默认：

AppId + "-" +  AppIdSuffix

AppId默认：Appid.Replace(".","-")

AppIdSuffix默认：网卡地址

当AppIdSuffix为空时，dapr的appid等于AppId