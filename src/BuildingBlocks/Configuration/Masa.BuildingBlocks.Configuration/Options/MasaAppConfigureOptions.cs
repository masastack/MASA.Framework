// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class MasaAppConfigureOptions
{
    public string AppId { get; set; }

    public string Environment { get; set; }

    public string Cluster { get; set; }

    public Dictionary<string, string> Data { get; set; } = new();
}
