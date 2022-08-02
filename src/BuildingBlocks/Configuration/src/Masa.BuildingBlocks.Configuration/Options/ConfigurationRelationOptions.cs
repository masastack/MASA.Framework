// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class ConfigurationRelationOptions
{
    public SectionTypes SectionType { get; set; }

    public string? ParentSection { get; set; }

    public string? Section { get; set; } = default!;

    /// <summary>
    /// Object type of mapping node relationship
    /// </summary>
    public Type ObjectType { get; set; } = default!;
}
