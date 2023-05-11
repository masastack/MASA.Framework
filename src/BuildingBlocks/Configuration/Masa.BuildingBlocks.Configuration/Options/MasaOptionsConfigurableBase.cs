// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public abstract class MasaOptionsConfigurableBase : IMasaOptionsConfigurable
{
    /// <summary>
    /// The name of the parent section, if it is empty, it will be mounted under SectionType, otherwise it will be mounted to the specified section under SectionType
    /// </summary>
    [JsonIgnore]
    public virtual string? ParentSection => null;

    /// <summary>
    /// The section null means same as the class name, else load from the specify section
    /// </summary>
    [JsonIgnore]
    public virtual string? Section => null;

    [JsonIgnore]
    public abstract SectionTypes SectionType { get; }

    /// <summary>
    /// The name of the options instance
    /// Normally it is string.Empty
    /// </summary>
    [JsonIgnore]
    public virtual string Name => string.Empty;
}
