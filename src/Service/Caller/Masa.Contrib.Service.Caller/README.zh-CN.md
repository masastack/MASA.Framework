中 | [EN](README.md)

## Masa.Contrib.Service.Caller

Masa.Contrib.Service.Caller是Caller的基础类库，提供了以下能力的抽象

* `ICallerFactory`: 工厂，用于创建`CallerProvider` (Singleton)
* `ICallerProvider`: 提供`Post`、`Delete`、`Patch`、`Put`、`Get`、`Send`的能力 (Scoped)
* `IRequestMessage`: 提供对请求数据处理的能力 (默认实现[`JsonRequestMessage`](./JsonRequestMessage.cs)) (Singleton)
* `IResponseMessage`: 提供对响应数据处理的能力 (默认实现[`DefaultResponseMessage`](./DefaultResponseMessage.cs)) (Singleton)
* `ITypeConvertProvider`: 提供类型转换的能力，为`ICallerProvider`的`Get`请求支撑 (Singleton)

## 总结

`Masa.Contrib.Service.Caller`是Caller的基础类库，但不能单独使用，目前Caller支持了两种实现方式：

* 基于HttpClient的实现: [Masa.Contrib.Service.Caller.HttpClient](../Masa.Contrib.Service.Caller.HttpClient/README.zh-CN.md)
* 基于DaprClient的实现: [Masa.Contrib.Service.Caller.DaprClient](../Masa.Contrib.Service.Caller.DaprClient/README.zh-CN.md)

> Q: 如果被调用方使用的是数据格式为xml，而不是json，我应该怎么做？
>
> A: 重写IRequestMessage，在调用AddCaller之前先将自定义的RequestMessage添加到IServiceCollection中

  ``` C#
  services.AddSingleton<IRequestMessage, XmlRequestMessage>();
  services.AddCaller();
  ```

> Q: 如果希望处理自定义的StatusCode，并抛出异常信息
>
> A: 重写IResponseMessage，在调用AddCaller之前先将自定义的ResponseMessage添加到IServiceCollection中

  ``` C#
  services.AddSingleton<IResponseMessage, CustomizeResponseMessage>();
  services.AddCaller();
  ```