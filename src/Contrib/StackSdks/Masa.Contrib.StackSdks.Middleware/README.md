[中](README.zh-CN.md) | EN

## Masa.Contrib.StackSdks.Middleware

Example:

```C#
Install-Package Masa.Contrib.StackSdks.Middleware
```

```C#
builder.Services.AddStackMiddleware();

xxxxxxxx

app.UseAddStackMiddleware();
```

`Masa.Contrib.StackSdks.Middleware` provides default implementations of `IDisabledEventDeterminer` and `IDisabledRequestDeterminer`, relying on `Masa.Contrib.Authentication.Identity`, `Masa.Contrib.StackSdks.Config`. The initialization of the response code should be done before `AddStackMiddleware`.

You can rewrite `IDisabledEventDeterminer` and `IDisabledRequestDeterminer` to replace the default implementation according to business needs.

```C#
builder.Services.AddScoped<IDisabledEventDeterminer, CustomizeDisabledEventDeterminer>();

builder.Services.AddStackMiddleware();
```

### DisabledEventMiddleware

According to the return value of the `Determiner` method of `IDisabledRequestDeterminer`, the attribute `DisabledRouteAttribute` is determined, and the request whose routing metadata contains this attribute is disabled.

### DisabledRequestMiddleware

According to the return value of the `Determiner` method of `IDisabledEventDeterminer` and the `DisabledCommand` attribute (the default implementation is true, which disables all Commands), it is really necessary to add the `AllowedEventAttribute` feature to the release implementation.
