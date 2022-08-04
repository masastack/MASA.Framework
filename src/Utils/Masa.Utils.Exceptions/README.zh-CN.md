中 | [EN](README.md)

## Masa.Utils.Exceptions

提供了用于处理Web应用程序异常的模型

* 支持自定义处理异常，用于处理非Masa提供的异常
* 接管`UserFriendlyException`异常，并响应状态码为299，返回友好的错误信息
* 默认处理所有异常，并对外输出`An error occur in masa framework`

## 用例:

``` C#
Install-Package Masa.Utils.Exceptions
```

1. 修改`Program.cs`

``` C#
app.UseMasaExceptionHandler();
```

2. 如何使用？

``` C#
app.MapGet("/Test", ()
{
    throw new UserFriendlyException("This method is deprecated");
}
```

3. 错误响应消息，其中Http状态码为299

``` js
axios
    .get('/Test')
    .then(response => {
        if (response.status === 299) {
            alert(response.data);
        }
    })
```

## 如何自定义异常处理？

1. 通过指定`ExceptionHandler`

    ``` C#
    app.UseMasaExceptionHandler(option =>
    {
        option.CatchAllException = true;//是否捕获所有异常，默认为true，捕获到的异常默认输出：An error occur in masa framework

        // 自定义处理异常，与ExceptionFilter类似，可根据异常类型处理异常信息，并通过ToResult方法输出响应结果
        option.ExceptionHandler = context =>
        {
            if (context.Exception is ArgumentNullException argumentNullException)
            {
                context.ExceptionHandled = true;
                context.Message = "参数不能为空";
                // 或者简写为context.ToResult("参数不能为空");
            }
        };
    });
    ```

2. 实现`IExceptionHandler`接口，并注册到服务中

    ``` C#
    public class ExceptionHandler : IMasaExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void OnException(MasaExceptionContext context)
        {
            if (context.Exception is ArgumentNullException)
            {
                _logger.LogWarning(context.Message);
                context.ToResult(context.Exception.Message);
            }
        }
    }
    builder.Services.AddSingleton<ExceptionHandler>();

    app.UseMasaExceptionHandler();
    ```

3. 实现`IExceptionHandler`接口，并指定使用Handler

    ``` C#
    public class ExceptionHandler : IMasaExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void OnException(MasaExceptionContext context)
        {
            if (context.Exception is ArgumentNullException)
            {
                _logger.LogWarning(context.Message);
                context.ToResult(context.Exception.Message);
            }
        }
    }
    app.UseMasaExceptionHandler(option =>
    {
        option.UseExceptionHanlder<ExceptionHandler>();
    });
    ```

## 常见问题

默认`UserFriendlyException`的日志等级为`Information`, 其它类型异常为`Error`

1. 如何修改UserFriendlyException的日志等级？

``` C#
builder.Services.Configure<MasaExceptionLogRelationOptions>(options =>
{
    options.MapLogLevel<UserFriendlyException>(LogLevel.None);
});
```