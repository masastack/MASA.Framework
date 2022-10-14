// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Builder;

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
                var masaExceptionHandler =
                    Masa.Utils.Exceptions.Internal.ExceptionHandlerExtensions.GetMasaExceptionHandler(
                        serviceProvider,
                        _options.MasaExceptionHandlerType);
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
