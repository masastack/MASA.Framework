// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

internal class IsolationConfigurationApi : IConfigurationApi
{
    readonly IConfiguration _configuration;
    readonly IHttpContextAccessor _httpContextAccessor;

    public IsolationConfigurationApi(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public IConfiguration Get(string appId)
    {
        var multiEnvironmentContext = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IMultiEnvironmentContext>();
        var environment = multiEnvironmentContext.CurrentEnvironment;
        if (environment.IsNullOrEmpty())
        {
            var multiEnvironmentUserContext = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IMultiEnvironmentUserContext>();
            environment = multiEnvironmentUserContext.Environment;
        }
        if (environment.IsNullOrEmpty())
        {
            return _configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection(appId);
        }
        return _configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection(environment).GetSection(appId);
    }
}
