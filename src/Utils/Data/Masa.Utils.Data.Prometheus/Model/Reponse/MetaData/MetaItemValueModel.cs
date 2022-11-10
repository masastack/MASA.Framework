// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Model;

public class MetaItemValueModel
{
    /// <summary>
    /// metric's data type, like counter、gauge、histogram and more
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// metric's description ,like "Cpu usage in seconds"
    /// </summary>
    public string Help { get; set; }

    /// <summary>
    /// metric's data unit
    /// </summary>
    public string Unit { get; set; }
}
