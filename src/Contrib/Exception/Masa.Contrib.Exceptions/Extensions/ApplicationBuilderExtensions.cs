// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Use I18n <see cref="ExceptionHandlerMiddleware"/>
    /// </summary>
    /// <param name="app"></param>
    /// <param name="exceptionHandlingOptions"></param>
    /// <param name="defaultCulture"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMasaExceptionHandler(
        this IApplicationBuilder app,
        Action<MasaExceptionHandlerOptions>? exceptionHandlingOptions = null)
    {
        var option = new MasaExceptionHandlerOptions();
        exceptionHandlingOptions?.Invoke(option);

        app.UseMiddleware<ExceptionHandlerMiddleware>(Options.Create(option));

        return app;
    }
}
