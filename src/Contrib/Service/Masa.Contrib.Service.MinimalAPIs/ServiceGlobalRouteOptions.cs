// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs;

public class ServiceGlobalRouteOptions : ServiceRouteOptions
{
    public Assembly[] Assemblies { get; set; }

    public Action<RouteHandlerBuilder>? RouteHandlerBuilder { get; set; }

    internal PluralizationService Pluralization { get; set; }

    public ServiceGlobalRouteOptions()
    {
        DisableRestful = false;
        Prefix = "api";
        Version = "v1";
        AutoAppendId = true;
        PluralizeServiceName = true;
        GetPrefixs = new[] { "Get", "Select" };
        PostPrefixs = new[] { "Post", "Add", "Upsert", "Create" };
        PutPrefixs = new[] { "Put", "Update", "Modify" };
        DeletePrefixs = new[] { "Delete", "Remove" };
        Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Pluralization = PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en"));
    }
}
