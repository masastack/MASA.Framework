// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Exceptions.Handlers;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Use localizable <see cref="ExceptionHandlingMiddleware"/>
    /// </summary>
    /// <param name="app"></param>
    /// <param name="exceptionHandlingOptions"></param>
    /// <returns></returns>
    [Obsolete("UseMasaExceptionHandler is recommended to use instead.")]
    public static IApplicationBuilder UseMasaExceptionHandling(
        this IApplicationBuilder app,
        Action<MasaExceptionHandlingOptions>? exceptionHandlingOptions = null)
    {
        return app.UseMasaExceptionHandling(_ =>
        {
        }, exceptionHandlingOptions);
    }

    /// <summary>
    /// Use localizable <see cref="ExceptionHandlingMiddleware"/>
    /// </summary>
    /// <param name="app"></param>
    /// <param name="action"></param>
    /// <param name="exceptionHandlingOptions"></param>
    /// <returns></returns>
    [Obsolete("UseMasaExceptionHandler is recommended to use instead.")]
    public static IApplicationBuilder UseMasaExceptionHandling(
        this IApplicationBuilder app,
        Action<RequestLocalizationOptions> action,
        Action<MasaExceptionHandlingOptions>? exceptionHandlingOptions)
    {
        var option = new MasaExceptionHandlingOptions();
        exceptionHandlingOptions?.Invoke(option);

        app.UseMiddleware<ExceptionHandlingMiddleware>(Options.Create(option));
        app.UseRequestLocalization(action);
        return app;
    }

    /// <summary>
    /// Use localizable <see cref="ExceptionHandlingMiddleware"/>
    /// </summary>
    /// <param name="app"></param>
    /// <param name="exceptionHandlingOptions"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMasaExceptionHandler(
        this IApplicationBuilder app,
        Action<MasaExceptionHandlerOptions>? exceptionHandlingOptions = null)
    {
        return app.UseMasaExceptionHandler(_ =>
        {
        }, exceptionHandlingOptions);
    }

    /// <summary>
    /// Use localizable <see cref="ExceptionHandlingMiddleware"/>
    /// </summary>
    /// <param name="app"></param>
    /// <param name="action"></param>
    /// <param name="exceptionHandlingOptions"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMasaExceptionHandler(
        this IApplicationBuilder app,
        Action<RequestLocalizationOptions> action,
        Action<MasaExceptionHandlerOptions>? exceptionHandlingOptions)
    {
        var option = new MasaExceptionHandlerOptions();
        exceptionHandlingOptions?.Invoke(option);

        app.UseMiddleware<ExceptionHandlerMiddleware>(Options.Create(option));
        app.UseRequestLocalization(action);
        return app;
    }
}
