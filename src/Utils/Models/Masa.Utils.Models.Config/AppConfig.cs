// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Models.Config;

/// <summary>
/// Apply global configuration
/// </summary>
public class AppConfig
{
    public string? AppId { get; set; }

    public string? Environment { get; set; }

    public string? Cluster { get; set; }
}
