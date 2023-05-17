// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class ConfigurationAutoMapOptions
{
    /// <summary>
    /// List of configurations that support options mode
    /// </summary>
    public List<ConfigurationRelationOptions> Data { get; set; } = new();
}
