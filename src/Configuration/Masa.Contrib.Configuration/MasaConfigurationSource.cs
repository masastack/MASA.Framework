// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class MasaConfigurationSource : IConfigurationSource
{
    internal readonly MasaConfigurationBuilder? Builder;

    internal readonly IConfigurationProvider? ConfigurationProvider;

    public MasaConfigurationSource(MasaConfigurationBuilder builder) => Builder = builder;

    public MasaConfigurationSource(IConfigurationProvider configurationProvider) => ConfigurationProvider = configurationProvider;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => Builder != null ? new MasaConfigurationProvider(this) : ConfigurationProvider!;
}
