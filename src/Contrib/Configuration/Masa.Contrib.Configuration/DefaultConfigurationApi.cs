// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

[ExcludeFromCodeCoverage]
public class DefaultConfigurationApi : IConfigurationApi
{
    private readonly IMasaConfiguration _masaConfiguration;

    public DefaultConfigurationApi(IMasaConfiguration masaConfiguration)
    {
        _masaConfiguration = masaConfiguration;
    }

    public IConfiguration Get(string appId)
    {
        return _masaConfiguration
            .GetConfiguration(SectionTypes.ConfigurationApi)
            .GetSection(appId);
    }
}
