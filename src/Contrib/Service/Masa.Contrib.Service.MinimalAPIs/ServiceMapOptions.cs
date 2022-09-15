// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs;

public class ServiceMapOptions : ServiceBaseOptions
{
    public Assembly[] Assemblies { get; set; }

    internal PluralizationService Pluralization { get; set; }

    public ServiceMapOptions()
    {
        Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Prefix = "api";
        Version = "v1";
        PluralizeServiceName = false;
        GetPrefixs = new[] { "GET", "SELECT" };
        PostPrefixs = new[] { "POST", "ADD", "Upsert" };
        PutPrefixs = new[] { "PUT", "Update", "Modify" };
        DeletePrefixs = new[] { "DELETE", "REMOVE" };
        Pluralization = PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en"));
    }
}
