// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    private static bool _isInitialize;

    public static IApplicationBuilder UseI18N(this IApplicationBuilder app, string? defaultCulture = null)
    {
        if (_isInitialize)
            return app;

        _isInitialize = true;
        var settings = app.ApplicationServices.GetRequiredService<IOptions<CultureSettings>>().Value;

        var requestLocalization = new RequestLocalizationOptions();

        var cultures = settings.SupportedCultures.Select(x => x.Culture).ToArray();
        requestLocalization
            .AddSupportedCultures(cultures)
            .AddSupportedUICultures(cultures);
        requestLocalization.SetDefaultCulture(!string.IsNullOrWhiteSpace(defaultCulture) ? defaultCulture : cultures.FirstOrDefault()!);

        requestLocalization.ApplyCurrentCultureToResponseHeaders = true;
        app.UseRequestLocalization(requestLocalization);
        return app;
    }
}
