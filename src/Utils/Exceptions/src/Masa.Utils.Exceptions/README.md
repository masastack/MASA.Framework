[中](README.zh-CN.md) | EN

## Masa.Utils.Exceptions

Provides a model for handling web application exceptions

* Support custom handling exceptions for handling exceptions not provided by Masa
* Take over the `UserFriendlyException` exception and respond with a status code of 299 and return a friendly error message
* Handle all exceptions by default, and output `An error occur in masa framework` externally

## Example:

``` C#
Install-Package Masa.Utils.Exceptions
```

1. Modify `Program.cs`

``` C#
app.UseMasaExceptionHandler();
```

2. How to use?

``` C#
app.MapGet("/Test", ()
{
    throw new UserFriendlyException("This method is deprecated");
}
```

3. Error response message, where Http status code is 299

``` js
axios
    .get('/Test')
    .then(response => {
        if (response.status === 299) {
            alert(response.data);
        }
    })
```

## How to customize exception handling?

1. By specifying `ExceptionHandler`

    ```` C#
    app.UseMasaExceptionHandler(option =>
    {
        option.CatchAllException = true;//Whether to catch all exceptions, the default is true, the default output of caught exceptions: An error occur in masa framework

        // Custom handling exceptions, similar to ExceptionFilter, can handle exception information according to the exception type, and output the response result through the ToResult method
        option.ExceptionHandler = context =>
        {
            if (context.Exception is ArgumentNullException argumentNullException)
            {
                context.ExceptionHandled = true;
                context.Message = "Parameter cannot be empty";
                // or abbreviated as: context.ToResult("Parameter cannot be empty");
            }
        };
    });
    ````

2. Implement the `IExceptionHandler` interface and register it with the service

    ```` C#
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
    ````

3. Implement the `IExceptionHandler` interface and specify the use of Handler

    ```` C#
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
    ````

## Common problem

The default log level of `UserFriendlyException` is `Information`, other types of exceptions are `Error`

1. How to modify the log level of UserFriendlyException?

     ```` C#
     builder.Services.Configure<MasaExceptionLogRelationOptions>(options =>
     {
         options.MapLogLevel<UserFriendlyException>(LogLevel.None);
     });
     ````