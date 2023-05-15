// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static void MapMasaMinimalAPIs(this WebApplication webApplication)
    {
        MasaApp.Build(webApplication.Services);
        GlobalMinimalApiOptions.WebApplication = webApplication;

        var serviceMapOptions = webApplication.Services.GetRequiredService<IOptions<ServiceGlobalRouteOptions>>().Value;
        foreach (var serviceType in GlobalMinimalApiOptions.ServiceTypes)
        {
            var serviceInstance = (ServiceBase)webApplication.Services.GetRequiredService(serviceType);
            if (serviceInstance.RouteOptions.DisableAutoMapRoute ?? serviceMapOptions.DisableAutoMapRoute ?? false)
                continue;

            serviceInstance.AutoMapRoute(serviceMapOptions, serviceMapOptions.Pluralization);
        }
    }
}
