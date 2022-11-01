// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Configuration;

public class MasaAppConfigureOptionsRelation
{
    private Dictionary<string, (string Variable, string DefaultValue)> Data { get; set; }

    public MasaAppConfigureOptionsRelation()
    {
        Data = new Dictionary<string, (string Variable, string DefaultValue)>(StringComparer.OrdinalIgnoreCase)
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

    public MasaAppConfigureOptionsRelation SetOptionsRelation(string key, string variable, string defaultValue)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(key);

        MasaArgumentException.ThrowIfNullOrEmpty(variable);

        MasaArgumentException.ThrowIfNullOrEmpty(defaultValue);

        Data[key] = (variable, defaultValue);
        return this;
    }

    internal string[] GetKeys() => Data.Select(kvp => kvp.Key).ToArray();

    internal (string Variable, string DefaultValue) GetValue(string key)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(key);

        return Data[key];
    }
}
