// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class MasaAppConfigureOptions
{
    public string AppId { get => GetValue(nameof(AppId)); set => Data[nameof(AppId)] = value; }

    public string Environment { get => GetValue(nameof(Environment)); set => Data[nameof(Environment)] = value; }

    public string Cluster { get => GetValue(nameof(Cluster)); set => Data[nameof(Cluster)] = value; }

    public Dictionary<string, string> Data { get; set; } = new();

    private string GetValue(string key)
    {
        if (Data.ContainsKey(key)) return Data[key];

        return string.Empty;
    }
}
