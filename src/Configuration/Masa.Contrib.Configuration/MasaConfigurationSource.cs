// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class MasaConfigurationSource : IConfigurationSource
{
    internal readonly MasaConfigurationBuilder Builder;

    public MasaConfigurationSource(MasaConfigurationBuilder builder) => Builder = builder;

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new MasaConfigurationProvider(this);
}
