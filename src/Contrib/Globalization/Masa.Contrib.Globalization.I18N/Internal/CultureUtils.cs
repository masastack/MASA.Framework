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
    start:
        try
        {
            var fileName = Path.Combine(languageDirectory, supportCultureName);
            using var fileStream = new FileStream(fileName, FileMode.Open);
            if (!fileStream.CanRead)
            {
                retry++;
                if (retry <= 10)
                {
                    Task.Delay(300);
                    goto start;
                }
                throw new FileLoadException("Failed to get supported languages", fileName);
            }
        }
        catch (FileNotFoundException ex)
        {
            // //todo: Write a log, prompting to use English by default
            return new List<CultureModel>()
            {
                new("en-us", "English")
            };
        }
        catch (IOException ex)
        {
            retry++;
            if (retry <= 10)
            {
                Task.Delay(300);
                goto start;
            }
            throw;
        }

        var supportCultureFilePath = Path.Combine(languageDirectory, supportCultureName);
        var content = File.ReadAllText(supportCultureFilePath);
        return System.Text.Json.JsonSerializer.Deserialize<List<CultureModel>>(content)!;
    }
}
