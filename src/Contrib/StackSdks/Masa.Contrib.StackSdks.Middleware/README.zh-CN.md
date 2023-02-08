中 | [EN](README.md)

## Masa.Contrib.StackSdks.Middleware

用例：

```C#
Install-Package Masa.Contrib.StackSdks.Middleware
```

```C#
builder.Services.AddStackMiddleware();

xxxxxxxx

app.UseAddStackMiddleware();
```

`Masa.Contrib.StackSdks.Middleware`提供了`IDisabledEventDeterminer`和`IDisabledRequestDeterminer`的默认实现，依赖`Masa.Contrib.Authentication.Identity`,`Masa.Contrib.StackSdks.Config`。`AddStackMiddleware`前应完成相应代码的初始化。

可以根据业务需要重写`IDisabledEventDeterminer`和`IDisabledRequestDeterminer`替换默认实现。

```C#
builder.Services.AddScoped<IDisabledEventDeterminer,CustomizeDisabledEventDeterminer>();

builder.Services.AddStackMiddleware();
```

### DisabledEventMiddleware

根据`IDisabledRequestDeterminer`的`Determiner`方法返回值，判断特性`DisabledRouteAttribute`，路由元数据包含该特性的请求禁用。

### DisabledRequestMiddleware

根据`IDisabledEventDeterminer`的`Determiner`方法返回值和`DisabledCommand`属性(默认实现为true,禁用所有Command),真的特殊需要放行实现添加`AllowedEventAttribute`特性。
