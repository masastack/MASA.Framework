// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class MasaAppConfigureOptionsRelation
{
    public Dictionary<string, string> DataVariables { get; }

    public Dictionary<string, string> DataDefaultValue { get; }

    public MasaAppConfigureOptionsRelation()
    {
        DataVariables = new()
        {
            { nameof(MasaAppConfigureOptions.AppId), nameof(MasaAppConfigureOptions.AppId) },
            { nameof(MasaAppConfigureOptions.Environment), "ASPNETCORE_ENVIRONMENT" },
            { nameof(MasaAppConfigureOptions.Cluster), nameof(MasaAppConfigureOptions.Cluster) },
        };
        DataDefaultValue = new Dictionary<string, string>()
        {
            {
                nameof(MasaAppConfigureOptions.AppId),
                (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", "-")
            },
            { nameof(MasaAppConfigureOptions.Environment), "Production" },
            { nameof(MasaAppConfigureOptions.Cluster), "Default" },
        };
    }
}
