// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public abstract class CustomServiceBase : ServiceBase
{
    protected CustomServiceBase()
    {
    }

    protected CustomServiceBase(string baseUri) : base(baseUri)
    {
    }

#pragma warning disable CS0618
    public CustomServiceBase(IServiceCollection services, string baseUri) : base(services, baseUri)
    {

    }
#pragma warning restore CS0618

    public string TestGetBaseUri(ServiceRouteOptions globalOptions)
        => base.GetBaseUri(globalOptions, PluralizationService.CreateService(System.Globalization.CultureInfo.CreateSpecificCulture("en")));

    public string TestGetMethodName(MethodInfo methodInfo, string prefix, ServiceRouteOptions globalOptions)
        => base.GetMethodName(methodInfo, prefix, globalOptions);

    public (string? HttpMethod, string Prefix) TestParseMethod(ServiceRouteOptions globalOptions, string methodName)
        => base.ParseMethod(globalOptions, methodName);

    public string[] TestGetDefaultHttpMethods(ServiceRouteOptions globalOptions)
        => base.GetDefaultHttpMethods(globalOptions);

    public string TestGetServiceName(PluralizationService? pluralizationService)
        => base.GetServiceName(pluralizationService);
}
