// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class MasaAppConfigureOptionsRelation
{
    public Dictionary<string, (string Variable, string DefaultValue)> Data { get; }

    public MasaAppConfigureOptionsRelation()
    {
        Data = new Dictionary<string, (string Variable, string DefaultValue)>()
        {
            {
                nameof(MasaAppConfigureOptions.AppId),
                (nameof(MasaAppConfigureOptions.AppId),
                    (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", "-"))
            },
            {
                nameof(MasaAppConfigureOptions.Environment),
                ("ASPNETCORE_ENVIRONMENT", "Production")
            },
            {
                nameof(MasaAppConfigureOptions.Cluster),
                (nameof(MasaAppConfigureOptions.Cluster), "Default")
            }
        };
    }

    public void SetData(Dictionary<string, (string Variable, string DefaultValue)> datas)
    {
        foreach (var data in datas)
        {
            Data[data.Key] = data.Value;
        }
    }
}
