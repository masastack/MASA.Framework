// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public abstract class ConfigurationApiMasaOptionsConfigurable : MasaOptionsConfigurableBase
{
    /// <summary>
    /// The name of the parent section, if it is empty, it will be mounted under SectionType, otherwise it will be mounted to the specified section under SectionType
    /// </summary>
    protected sealed override string? ParentSection => AppId;

    protected virtual string? AppId => null;

    /// <summary>
    /// The section null means same as the class name, else load from the specify section
    /// </summary>
    protected sealed override string? Section => ObjectName;

    protected virtual string? ObjectName => null;

    /// <summary>
    /// Configuration object name
    /// </summary>
    protected sealed override SectionTypes SectionType => SectionTypes.ConfigurationApi;
}
