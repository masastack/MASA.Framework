// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Globalization.I18N;

internal static class PathHelper
{
    internal static string GetAndCheckLanguageDirectory(string languageDirectory)
    {
        languageDirectory.CheckIsNullOrWhiteSpace();
        var path = Path.Combine(I18NResourceResourceConfiguration.BaseDirectory, languageDirectory.TrimStart("/"));
        if (!Directory.Exists(path))
        {
            throw new UserFriendlyException($"[{languageDirectory}] does not exist");
        }

        return path;
    }
}
