// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public abstract class MasaOptionsConfigurableBase : IMasaOptionsConfigurable
{
    /// <summary>
    /// The name of the parent section, if it is empty, it will be mounted under SectionType, otherwise it will be mounted to the specified section under SectionType
    /// </summary>
    [JsonIgnore]
    protected virtual string? ParentSection => null;

    /// <summary>
    /// The section null means same as the class name, else load from the specify section
    /// </summary>
    protected virtual string? Section => null;

    protected abstract SectionTypes SectionType { get; }

    /// <summary>
    /// The name of the options instance
    /// Normally it is string.Empty
    /// </summary>
    protected virtual string OptionsName => string.Empty;

    public string? GetParentSection() => ParentSection;

    public string? GetSection() => Section;

    public SectionTypes GetSectionType() => SectionType;

    public string GetOptionsName() => OptionsName;
}
