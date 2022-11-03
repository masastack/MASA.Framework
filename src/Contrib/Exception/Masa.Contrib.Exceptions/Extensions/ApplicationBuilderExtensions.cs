// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Use localizable <see cref="ExceptionHandlerMiddleware"/>
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
    /// Use localizable <see cref="ExceptionHandlerMiddleware"/>
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
