[中](README.zh-CN.md) | EN

## Masa.Contrib.BasicAbility.Pm

Effect:

Obtain relevant data of Dcc service through DccClient,If you need to use the configuration related capabilities, please  refer to the[document](../../Configuration/Masa.Contrib.Configuration.ConfigurationApi.Dcc/README.zh-CN.md)

```c#
IDccClient
├── LabelService                  Label service
```

Example：

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

How to use：

```c#
var app = builder.Build();

app.MapGet("/GetProjectTypes", ([FromServices] IDccClient dccClient, string typeCode) =>
{
    return dccClient.LabelService.GetListByTypeCodeAsync(typeCode);
});

app.Run();
```
