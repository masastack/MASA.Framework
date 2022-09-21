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
        DisableAutoMapRoute = false;
        Prefix = "api";
        Version = "v1";
        AutoAppendId = true;
        PluralizeServiceName = true;
        GetPrefixes = new List<string> { "Get", "Select", "Find" };
        PostPrefixes = new List<string> { "Post", "Add", "Upsert", "Create", "Insert" };
        PutPrefixes = new List<string> { "Put", "Update", "Modify" };
        DeletePrefixes = new List<string> { "Delete", "Remove" };
        DisableTrimStartMethodPrefix = false;
        Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Pluralization = PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en"));
    }
}
