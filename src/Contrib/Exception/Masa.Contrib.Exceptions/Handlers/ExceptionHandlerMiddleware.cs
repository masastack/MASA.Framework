// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

[ExcludeFromCodeCoverage]
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MasaExceptionHandlerOptions _options;
    private readonly MasaExceptionLogRelationOptions _logRelationOptions;
    private readonly ILogger<ExceptionHandlerMiddleware>? _logger;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        IOptions<MasaExceptionHandlerOptions> options,
        IOptions<MasaExceptionLogRelationOptions> logRelationOptions,
        ILogger<ExceptionHandlerMiddleware>? logger = null)
    {
        _next = next;
        _options = options.Value;
        _logRelationOptions = logRelationOptions.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
    {
        var frameworkI18n = serviceProvider.GetService<II18n<MasaFrameworkResource>>();
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            var masaExceptionContext = new MasaExceptionContext(exception, httpContext, serviceProvider);
            if (_options.ExceptionHandler != null)
            {
                _options.ExceptionHandler.Invoke(masaExceptionContext);
            }
            else
            {
                var masaExceptionHandler = serviceProvider.GetMasaExceptionHandler(_options.MasaExceptionHandlerType);
                masaExceptionHandler?.OnException(masaExceptionContext);
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

            _logger?.WriteLog(masaExceptionContext.Exception, _logRelationOptions);

            var httpStatusCode = masaExceptionContext.Exception.GetHttpStatusCode();

            if (masaExceptionContext.Exception is MasaException masaException)
            {
                await httpContext.Response.WriteTextAsync(httpStatusCode, masaException.GetLocalizedMessage());
            }
            else if (_options.CatchAllException)
            {
                string message = frameworkI18n == null ? ExceptionErrorCode.GetErrorMessage(ExceptionErrorCode.INTERNAL_SERVER_ERROR)! :
                    frameworkI18n[ExceptionErrorCode.INTERNAL_SERVER_ERROR];
                await httpContext.Response.WriteTextAsync(httpStatusCode, message);
            }
            else
            {
                throw;
            }
        }
    }
}
