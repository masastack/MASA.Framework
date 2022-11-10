// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18n;

internal static class PathUtils
{
    internal static bool ParseResourcesDirectory(ref string resourcesPath)
    {
        resourcesPath.CheckIsNullOrWhiteSpace();
        resourcesPath = Path.Combine(I18NResourceResourceConfiguration.BaseDirectory, resourcesPath.TrimStart("/"));
        return Directory.Exists(resourcesPath);
    }
}
