// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Exceptions.Handlers;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMasaExceptionHandler? _masaExceptionHandler;
    private readonly MasaExceptionHandlerOptions _options;
    private readonly MasaExceptionLogRelationOptions _logRelationOptions;
    private readonly ILogger<ExceptionHandlerMiddleware>? _logger;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        IServiceProvider serviceProvider,
        IOptions<MasaExceptionHandlerOptions> options,
        IOptions<MasaExceptionLogRelationOptions> logRelationOptions,
        ILogger<ExceptionHandlerMiddleware>? logger = null)
    {
        _next = next;
        _options = options.Value;
        _masaExceptionHandler = ExceptionHandlerExtensions.GetMasaExceptionHandler(serviceProvider, _options.MasaExceptionHandlerType);
        _logRelationOptions = logRelationOptions.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            var masaExceptionContext = new MasaExceptionContext(exception, httpContext);
            if (_options.ExceptionHandler != null)
            {
                _options.ExceptionHandler.Invoke(masaExceptionContext);
            }
            else if (_masaExceptionHandler != null)
            {
                _masaExceptionHandler.OnException(masaExceptionContext);
            }

            if (httpContext.Response.HasStarted)
                return;

            if (masaExceptionContext.ExceptionHandled)
            {
                await httpContext.Response.WriteTextAsync(
                    masaExceptionContext.StatusCode,
                    masaExceptionContext.Message ?? masaExceptionContext.Exception.Message,
                    masaExceptionContext.ContentType);
                return;
            }

            _logger?.WriteLog(masaExceptionContext.Exception,
                masaExceptionContext.Exception is UserFriendlyException ? LogLevel.Information : LogLevel.Error,
                _logRelationOptions);

            if (masaExceptionContext.Exception is UserFriendlyException)
            {
                await httpContext.Response.WriteTextAsync((int)MasaHttpStatusCode.UserFriendlyException,
                    masaExceptionContext.Exception.Message);
            }
            else if (masaExceptionContext.Exception is MasaException || _options.CatchAllException)
            {
                var message = Constant.DEFAULT_EXCEPTION_MESSAGE;
                await httpContext.Response.WriteTextAsync((int)HttpStatusCode.InternalServerError, message);
            }
            else
            {
                throw;
            }
        }
    }


}
