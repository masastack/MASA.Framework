// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Mvc pipeline exception filter to catch global exception
/// </summary>
[ExcludeFromCodeCoverage]
public class MvcGlobalExceptionFilter : IExceptionFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMasaExceptionHandler? _masaExceptionHandler;
    private readonly MasaExceptionHandlerOptions _options;
    private readonly MasaExceptionLogRelationOptions _logRelationOptions;
    private readonly I18N<MasaFrameworkResource>? _frameworkI18N;
    private readonly ILogger<MvcGlobalExceptionFilter>? _logger;

    public MvcGlobalExceptionFilter(IServiceProvider serviceProvider,
        IOptions<MasaExceptionHandlerOptions> options,
        IOptions<MasaExceptionLogRelationOptions> logRelationOptions,
        I18N<MasaFrameworkResource>? frameworkI18N = null,
        ILogger<MvcGlobalExceptionFilter>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _masaExceptionHandler = serviceProvider.GetMasaExceptionHandler(_options.MasaExceptionHandlerType);
        _logRelationOptions = logRelationOptions.Value;
        _frameworkI18N = frameworkI18N;
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var masaExceptionContext = new MasaExceptionContext(context.Exception, context.HttpContext, _serviceProvider);
        if (_options.ExceptionHandler != null)
        {
            _options.ExceptionHandler.Invoke(masaExceptionContext);
        }
        else
        {
            _masaExceptionHandler?.OnException(masaExceptionContext);
        }

        if (masaExceptionContext.HttpContext.Response.HasStarted)
            return;

        if (masaExceptionContext.ExceptionHandled)
        {
            context.ExceptionHandled = true;
            context.Result = new DefaultExceptionResult(
                masaExceptionContext.Message!,
                masaExceptionContext.StatusCode,
                masaExceptionContext.ContentType);
            return;
        }

        _logger?.WriteLog(masaExceptionContext.Exception, _logRelationOptions);

        var httpStatusCode = masaExceptionContext.Exception.GetHttpStatusCode();

        if (masaExceptionContext.Exception is MasaException masaException)
        {
            context.ExceptionHandled = true;
            context.Result = new DefaultExceptionResult(masaException.GetLocalizedMessage(),
                httpStatusCode,
                masaExceptionContext.ContentType);
            return;
        }
        if (_options.CatchAllException)
        {
            context.ExceptionHandled = true;

            string message = _frameworkI18N == null ? ErrorCode.GetErrorMessage(ErrorCode.INTERNAL_SERVER_ERROR)! :
                _frameworkI18N[ErrorCode.INTERNAL_SERVER_ERROR];
            context.Result = new DefaultExceptionResult(message,
                httpStatusCode,
                masaExceptionContext.ContentType);
            return;
        }
    }
}
