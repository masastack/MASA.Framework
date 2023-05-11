// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

[ExcludeFromCodeCoverage]
public abstract class LocalMasaOptionsConfigurable : MasaOptionsConfigurableBase
{
    /// <summary>
    /// ParentSection is not required for local configuration
    /// </summary>
    [JsonIgnore]
    public sealed override string? ParentSection => null;

    [JsonIgnore]
    public sealed override SectionTypes SectionType => SectionTypes.Local;
}
