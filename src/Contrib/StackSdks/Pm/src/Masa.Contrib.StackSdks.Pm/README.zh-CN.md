中 | [EN](README.md)

## Masa.Contrib.StackSdks.Pm

作用：

通过PmClient获取Pm服务的相关数据

```c#
IPmClient
├── EnvironmentService                  环境服务
├── ClusterService                      集群服务
├── ProjectService                      项目服务
├── AppService                          应用服务
```

用例：

```C#
Install-Package Masa.Contrib.StackSdks.Pm
```

```C#
builder.Services.AddPmClient("Pm服务地址");
```

如何使用：

```c#
var app = builder.Build();

app.MapGet("/GetProjectApps", ([FromServices] IPmClient pmClient) =>
{
    return pmClient.ProjectService.GetProjectAppsAsync("development");
});

app.Run();
```
