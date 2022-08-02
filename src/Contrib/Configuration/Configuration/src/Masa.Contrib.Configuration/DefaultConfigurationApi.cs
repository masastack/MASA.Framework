// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class DefaultConfigurationApi : IConfigurationApi
{
    private readonly IConfiguration _configuration;

    public DefaultConfigurationApi(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IConfiguration Get(string appId)
    {
        return _configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection(appId);
    }
}
