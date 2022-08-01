[中](README.zh-CN.md) | EN

## Masa.Contrib.BasicAbility.Pm

Effect:

Obtain relevant data of Pm service through pmclient

```c#
IPmClient
├── EnvironmentService                  Environment service
├── ClusterService                      Cluster service
├── ProjectService                      Project service
├── AppService                          App service
```

Example：

```C#
Install-Package Masa.Contrib.BasicAbility.Pm
```

```C#
builder.Services.AddPmClient("Pm service address");
```

How to use：

```c#
var app = builder.Build();

app.MapGet("/GetProjectApps", ([FromServices] IPmClient pmClient) =>
{
    return pmClient.ProjectService.GetProjectAppsAsync("development");
});

app.Run();
```
