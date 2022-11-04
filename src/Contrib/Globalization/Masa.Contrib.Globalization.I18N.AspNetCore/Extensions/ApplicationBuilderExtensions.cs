// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseI18N(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices.GetRequiredService<IOptions<CultureSettings>>().Value;

        var requestLocalization = new RequestLocalizationOptions();

        var cultures = settings.SupportedCultures.Select(x => x.Culture).ToArray();
        requestLocalization
            .AddSupportedCultures(cultures)
            .AddSupportedUICultures(cultures);
        if (!string.IsNullOrWhiteSpace(settings.DefaultCulture))
            requestLocalization.DefaultRequestCulture = new RequestCulture(settings.DefaultCulture);

        requestLocalization.ApplyCurrentCultureToResponseHeaders = true;
        app.UseRequestLocalization(requestLocalization);
        return app;
    }
}
