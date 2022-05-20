// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class DefaultMasaConfiguration : IMasaConfiguration
{
    private readonly IConfiguration _configuration;

    public DefaultMasaConfiguration(IConfiguration configuration) => _configuration = configuration;

    public IConfiguration GetConfiguration(SectionTypes sectionType) => _configuration.GetSection(sectionType.ToString());
}
