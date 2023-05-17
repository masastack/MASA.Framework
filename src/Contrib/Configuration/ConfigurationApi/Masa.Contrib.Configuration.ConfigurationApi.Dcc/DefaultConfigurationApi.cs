// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

[ExcludeFromCodeCoverage]
public class DefaultConfigurationApi : IConfigurationApi
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<IMasaConfiguration> _masaConfigurationLazy;

    public DefaultConfigurationApi(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _masaConfigurationLazy = new Lazy<IMasaConfiguration>(serviceProvider.GetRequiredService<IMasaConfiguration>);
    }

    public IConfiguration Get(string appId)
    {
        return _masaConfigurationLazy.Value
            .GetConfiguration(SectionTypes.ConfigurationApi)
            .GetSection(appId);
    }
}
