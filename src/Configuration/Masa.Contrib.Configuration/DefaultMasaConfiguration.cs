// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class DefaultMasaConfiguration : IMasaConfiguration
{
    private readonly IConfiguration _configuration;

    public IConfiguration Local => GetConfiguration(SectionTypes.Local);

    public IConfigurationApi ConfigurationApi { get; }

    public DefaultMasaConfiguration(IConfiguration configuration, IConfigurationApi configurationApi)
    {
        _configuration = configuration;
        ConfigurationApi = configurationApi;
    }

    public IConfiguration GetConfiguration(SectionTypes sectionType) => _configuration.GetSection(sectionType.ToString());
}
