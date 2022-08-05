// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Internal;

internal static class ConfigurationManagerExtensions
{
    public static string GetConfigurationValue(this ConfigurationManager configurationManager, string key, Func<string> func)
    {
        var configurationValue = configurationManager[key];
        return string.IsNullOrWhiteSpace(configurationValue) ? func() : configurationValue;
    }
}
