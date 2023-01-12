// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Service.Caller;

internal class CultureMiddleware : ICallerMiddleware
{
    public Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {
        var name = "cookie";
        if (masaHttpContext.RequestMessage.Headers.TryGetValues(name, out IEnumerable<string>? cookieValues))
        {
            if (IsExistCulture(cookieValues)) return next();

            masaHttpContext.RequestMessage.Headers.Remove(name);
        }

        var cookies = cookieValues?.ToList() ?? new List<string>();
        cookies.Add($".AspNetCore.Culture={GetCultureValue()}");
        masaHttpContext.RequestMessage.Headers.Add(name, cookies);
        return next();
    }

    private static bool IsExistCulture(IEnumerable<string>? cookieValues)
        => cookieValues != null && cookieValues.Any(cookie => cookie.Contains(".AspNetCore.Culture=", StringComparison.OrdinalIgnoreCase));

    private static string GetCultureValue() => GetCultureValue(new (string Key, string Value)[]
    {
        ("c", CultureInfo.CurrentCulture.Name),
        ("uic", CultureInfo.CurrentUICulture.Name)
    });

    private static string GetCultureValue((string Key, string Value)[] cultures)
        => System.Web.HttpUtility.UrlEncode(string.Join('|', cultures.Select(c => $"{c.Key}={c.Value}")));
}
