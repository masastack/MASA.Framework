// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.Internal;

internal static class CultureUtils
{
    public static List<CultureModel> GetSupportedCultures(
        string languageDirectory,
        string supportCultureName)
    {
        int retry = 0;
        int maxRetry = 10;
        while (retry < maxRetry)
        {
            try
            {
                var supportCultureFilePath = Path.Combine(languageDirectory, supportCultureName);
                var content = File.ReadAllText(supportCultureFilePath);
                return System.Text.Json.JsonSerializer.Deserialize<List<CultureModel>>(content)!;
            }
            catch (FileNotFoundException)
            {
                return new List<CultureModel>()
                {
                    new("en-us", "English")
                };
            }
            catch (IOException)
            {
                if (retry == maxRetry - 1) throw;
            }
            finally
            {
                retry++;
            }
        }
        return new List<CultureModel>()
        {
            new("en-us", "English")
        };
    }
}
