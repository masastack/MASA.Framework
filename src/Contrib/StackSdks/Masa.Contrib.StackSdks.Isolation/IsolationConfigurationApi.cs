// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

internal class IsolationConfigurationApi : IConfigurationApi
{
    readonly IConfiguration _configuration;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IMasaStackConfig _stackConfig;

    public IsolationConfigurationApi(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _stackConfig = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IMasaStackConfig>();
    }

    public IConfiguration Get(string appId)
    {
        var multiEnvironmentContext = _httpContextAccessor.HttpContext?.RequestServices.GetService<IMultiEnvironmentContext>();
        var environment = multiEnvironmentContext?.CurrentEnvironment;
        if (environment.IsNullOrEmpty())
        {
            var multiEnvironmentUserContext = _httpContextAccessor.HttpContext?.RequestServices.GetService<IMultiEnvironmentUserContext>();
            environment = multiEnvironmentUserContext?.Environment;
        }

        if (environment.IsNullOrEmpty())
        {
            environment = _stackConfig.Environment;
        }
        if (environment.IsNullOrEmpty())
        {
            return _configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection(appId);
        }
        return _configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection(environment).GetSection(appId);
    }
}
