// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

[ExcludeFromCodeCoverage]
public class DefaultMasaConfiguration : IMasaConfiguration
{
    private readonly IConfiguration _configuration;
    private readonly Lazy<IConfigurationApi?> _configurationApiLazy;

    public IConfiguration Local => GetConfiguration(SectionTypes.Local);

    public IConfigurationApi? ConfigurationApi => _configurationApiLazy.Value;

    public DefaultMasaConfiguration(IConfiguration configuration, Lazy<IConfigurationApi?> configurationApiLazy)
    {
        _configuration = configuration;
        _configurationApiLazy = configurationApiLazy;
    }

    public IConfiguration GetConfiguration(SectionTypes sectionType) => _configuration.GetSection(sectionType.ToString());
}
