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
    /// <param name="useI18N">Whether to enable i18n</param>
    /// <returns></returns>
    public static IApplicationBuilder UseMasaExceptionHandler(
        this IApplicationBuilder app,
        Action<MasaExceptionHandlerOptions>? exceptionHandlingOptions = null,
        bool useI18N = true)
    {
        var option = new MasaExceptionHandlerOptions();
        exceptionHandlingOptions?.Invoke(option);

        if (useI18N) app.UseI18N();

        app.UseMiddleware<ExceptionHandlerMiddleware>(Options.Create(option));

        return app;
    }
}
