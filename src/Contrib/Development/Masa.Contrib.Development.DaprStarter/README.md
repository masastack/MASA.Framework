[中](README.zh-CN.md) | EN

## Masa.Contrib.Development.DaprStarter

Dapr Starter Core Library

Responsibilities:

Provide core support for Masa.Contrib.Development.DaprStarter.AspNetCore

The start, stop, refresh, and dapr daemon of the dapr process are provided by such libraries

Example：

``` powershell
Install-Package Masa.Contrib.Development.DaprStarter
```

### Get Started

1. Add DaprStarter to assist in managing the dapr process (recommended to be used in the development environment)

``` C#
builder.Services.AddDaprStarterCore();
```

2. Inject IDaprProcess at the specified location as needed, and then call the Start method to start the dapr process or hand it over to Masa.Contrib.Development.DaprStarter.AspNetCore to manage the dapr process. Related documents can be found at [View](../Masa.Contrib.Development.DaprStarter.AspNetCore/README.md)

Example：

New DaprController

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

### Advanced

#### Notice

1. The netstat command is used in the library, please make sure the netstat command is available

> Windows system supports netstat command by default without special installation
>
> Linux and OSX need to confirm by themselves whether netstat is installed on the computer

Open a terminal and enter the following command to confirm that the computer supports the netstat command:

````
netstat -h
````

Example: ubuntu:

````
apt-get install net-tools
````

2. AppId, AppIdSuffix strongly recommend not to enter a string containing ., otherwise it will cause problems with the dapr call. It is recommended to use -
    1. Dapr AppID follows the FQDN format, which includes the target namespace
    2. FQDN is spliced with the symbol .

#### Rule

dapr AppId naming rules default:

AppId + "-" + AppIdSuffix

AppId default: Appid.Replace(".","-")

AppIdSuffix default: network card address

When AppIdSuffix is empty, the appid of dapr is equal to AppId
