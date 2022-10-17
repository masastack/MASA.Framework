// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization.Dcc;

public class DccLocalizationResourceContributor : ILocalizationResourceContributor
{
    private readonly IConfigurationSection _configurationSection;

    public string CultureName { get; }

    public DccLocalizationResourceContributor(string appId, string configObject, IMasaConfiguration masaConfiguration)
    {
        _configurationSection = masaConfiguration.ConfigurationApi.Get(appId).GetSection(configObject);
    }

    public string? GetOrNull(string name)
    {
        if (_configurationSection.Exists())
        {
            return _configurationSection.GetValue<string>(name);
        }

        return null;
    }
}
