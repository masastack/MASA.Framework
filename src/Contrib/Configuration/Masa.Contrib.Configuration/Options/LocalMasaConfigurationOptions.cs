// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public abstract class LocalMasaConfigurationOptions : MasaConfigurationOptions
{
    /// <summary>
    /// ParentSection is not required for local configuration
    /// </summary>
    [JsonIgnore]
    public sealed override string? ParentSection => null;

    [JsonIgnore]
    public sealed override SectionTypes SectionType => SectionTypes.Local;
}
