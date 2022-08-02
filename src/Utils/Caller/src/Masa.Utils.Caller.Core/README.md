[中](README.zh-CN.md) | EN

## Masa.Utils.Caller.Core

Masa.Utils.Caller.Core is the basic class library of Caller, which provides the abstraction of the following capabilities

* `ICallerFactory`: Factory for creating `CallerProvider` (Singleton)
* `ICallerProvider`: Provides `Post`, `Delete`, `Patch`, `Put`, `Get`, `Send` capabilities (Scoped)
* `IRequestMessage`: Provides the ability to process request data (default implementation [`JsonRequestMessage`](./JsonRequestMessage.cs)) (Singleton)
* `IResponseMessage`: Provides the ability to handle response data (default implementation [`DefaultResponseMessage`](./DefaultResponseMessage.cs)) (Singleton)
* `ITypeConvertProvider`: Provides the ability to convert types, support for `Get` requests of `ICallerProvider` (Singleton)

## Summarize

`Masa.Utils.Caller.Core` is the basic class library of Caller, but it cannot be used alone. Currently, Caller supports two implementations:

* Implementation based on HttpClient: [Masa.Utils.Caller.HttpClient](../Masa.Utils.Caller.HttpClient/README.md)
* Implementation based on DaprClient: [Masa.Utils.Caller.DaprClient](../Masa.Utils.Caller.DaprClient/README.md)

> Q: What should I do if the callee uses xml instead of json?
>
> A: Rewrite IRequestMessage and add the custom RequestMessage to the IServiceCollection before calling AddCaller

  ```` C#
  services.AddSingleton<IRequestMessage, XmlRequestMessage>();
  services.AddCaller();
  ````

> Q: If you want to handle custom StatusCode and throw exception information
>
> A: Rewrite IResponseMessage, add custom ResponseMessage to IServiceCollection before calling AddCaller

  ```` C#
  services.AddSingleton<IResponseMessage, CustomizeResponseMessage>();
  services.AddCaller();
  ````