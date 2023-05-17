// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class ConfigurationRelationOptions
{
    /// <summary>
    /// Whether it is a required (required when initializing the configuration) configuration component
    /// Whether it is required (required when initializing the configuration) to configure the component, SectionType is not equal to Local
    /// default: false
    /// </summary>
    public bool IsRequiredConfigComponent { get; set; } = false;

    public SectionTypes SectionType { get; set; }

    public string? ParentSection { get; set; }

    public string? Section { get; set; } = default!;

    /// <summary>
    /// Object type of mapping node relationship
    /// </summary>
    public Type ObjectType { get; set; } = default!;

    public string OptionsName { get; set; } = string.Empty;
}
