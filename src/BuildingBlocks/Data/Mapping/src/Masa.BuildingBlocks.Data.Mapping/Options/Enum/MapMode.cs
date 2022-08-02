// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Mapping.Options.Enum;

/// <summary>
/// Mapping mode
/// Currently only shared mapping modes are supported
/// </summary>
public enum MapMode
{
    /// <summary>
    /// Use global settings and update global settings (update nested mappings)
    /// </summary>
    Shared = 1,
}
