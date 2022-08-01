中 | [EN](README.md)

## Masa.Contrib.BasicAbility.Dcc

作用：

通过DccClient获取Dcc服务的相关数据，如需使用配置相关能力请查看[文档](../../Configuration/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.zh-CN.md)

```c#
IDccClient
├── LabelService                  标签服务
```

用例：

```C#
Install-Package Masa.Contrib.BasicAbility.Dcc
```

appsettings.json

```json
{
  "DccOptions": {
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

```C#
builder.Services.AddDccClient();
```

如何使用：

```c#
var app = builder.Build();

app.MapGet("/GetProjectTypes", ([FromServices] IDccClient dccClient, string typeCode) =>
{
    return dccClient.LabelService.GetListByTypeCodeAsync(typeCode);
});

app.Run();
```
