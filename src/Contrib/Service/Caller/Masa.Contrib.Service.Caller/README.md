[中](README.zh-CN.md) | EN

## Masa.Contrib.Service.Caller

Masa.Contrib.Service.Caller is the basic class library of Caller, which provides the abstraction of the following capabilities

* `ICallerFactory`: Factory for creating `Caller` (Singleton)
* `ICaller`: Provides `Post`, `Delete`, `Patch`, `Put`, `Get`, `Send` capabilities (Scoped)
* `IRequestMessage`: Provides the ability to process request data (default implementation [`JsonRequestMessage`](./JsonRequestMessage.cs)) (Singleton)
* `IResponseMessage`: Provides the ability to handle response data (default implementation [`DefaultResponseMessage`](./DefaultResponseMessage.cs)) (Singleton)
* `ITypeConvertor`: Provides the ability to convert types, support for `Get` requests of `ICaller` (Singleton)

## Summarize

`Masa.Contrib.Service.Caller` is the basic class library of Caller, but it cannot be used alone. Currently, Caller supports two implementations:

* Implementation based on HttpClient: [Masa.Contrib.Service.Caller.HttpClient](../Masa.Contrib.Service.Caller.HttpClient/README.md)
* Implementation based on DaprClient: [Masa.Contrib.Service.Caller.DaprClient](../Masa.Contrib.Service.Caller.DaprClient/README.md)

> Q: What should I do if the callee uses xml instead of json?
>
> A: Rewrite IRequestMessage and add the custom RequestMessage to the IServiceCollection before calling AddCaller

  ``` C#
  services.AddSingleton<IRequestMessage, XmlRequestMessage>();
  services.AddCaller();
  ```

> Q: If you want to handle custom StatusCode and throw exception information
>
> A: Rewrite IResponseMessage, add custom ResponseMessage to IServiceCollection before calling AddCaller

  ``` C#
  services.AddSingleton<IResponseMessage, CustomResponseMessage>();
  services.AddCaller();
  ```