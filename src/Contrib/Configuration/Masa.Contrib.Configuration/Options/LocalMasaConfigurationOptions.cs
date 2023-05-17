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
    protected sealed override string? ParentSection => null;

    protected sealed override SectionTypes SectionType => SectionTypes.Local;
}
