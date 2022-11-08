// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18N.BlazorWebAssembly;

public static class CultureUtils
{
    public static async Task<List<CultureModel>> GetSupportedCulturesAsync(
        string baseAddress,
        string resourcePath,
        string supportCultureName)
    {
        var http = new HttpClient()
        {
            BaseAddress = new Uri(baseAddress)
        };
        List<CultureModel>? supportedCultures = null;
        try
        {
            var fileName = Path.Combine(resourcePath, supportCultureName);
            supportedCultures = await http.GetFromJsonAsync<List<CultureModel>>($"{fileName}");
        }
        catch (Exception ex)
        {
            //todo:
        }
        finally
        {
            supportedCultures ??= new List<CultureModel>()
            {
                new("en-us", "English")
            };
        }
        return supportedCultures;
    }
}
