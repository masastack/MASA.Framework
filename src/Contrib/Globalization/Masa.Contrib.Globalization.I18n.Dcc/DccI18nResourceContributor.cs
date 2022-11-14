// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Dcc;

public class DccI18nResourceContributor : II18nResourceContributor
{
    private readonly IConfigurationSection _configurationSection;

    public string CultureName { get; }

    public DccI18nResourceContributor(
        string appId,
        string configObjectPrefix,
        string cultureName,
        IMasaConfiguration masaConfiguration)
    {
        CultureName = cultureName;

        _configurationSection = masaConfiguration.ConfigurationApi.Get(appId).GetSection($"{configObjectPrefix}.{cultureName}");
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
