// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Mvc pipeline exception filter to catch global exception
/// </summary>
public class MvcGlobalExcetionFilter : IExceptionFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMasaExceptionHandler? _masaExceptionHandler;
    private readonly MasaExceptionHandlerOptions _options;
    private readonly MasaExceptionLogRelationOptions _logRelationOptions;
    private readonly ILogger<MvcGlobalExcetionFilter>? _logger;

    public MvcGlobalExcetionFilter(IServiceProvider serviceProvider,
        IOptions<MasaExceptionHandlerOptions> options,
        IOptions<MasaExceptionLogRelationOptions> logRelationOptions,
        ILogger<MvcGlobalExcetionFilter>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _masaExceptionHandler = ExceptionHandlerExtensions.GetMasaExceptionHandler(serviceProvider, _options.MasaExceptionHandlerType);
        _logRelationOptions = logRelationOptions.Value;
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
                masaExceptionContext.Message,
                masaExceptionContext.StatusCode,
                masaExceptionContext.ContentType);
            return;
        }

        _logger?.WriteLog(masaExceptionContext.Exception,
            masaExceptionContext.Exception is UserFriendlyException ? LogLevel.Information : LogLevel.Error,
            _logRelationOptions);

        if (masaExceptionContext.Exception is UserFriendlyException userFriendlyException)
        {
            context.ExceptionHandled = true;
            context.Result = new UserFriendlyExceptionResult(userFriendlyException.Message);
            return;
        }
        if (masaExceptionContext.Exception is MasaException || _options.CatchAllException)
        {
            context.ExceptionHandled = true;
            context.Result = new InternalServerErrorObjectResult(Constant.DEFAULT_EXCEPTION_MESSAGE);
            return;
        }
    }
}
