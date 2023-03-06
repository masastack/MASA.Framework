// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class MasaConfigurationSource : IConfigurationSource
{
    internal readonly MasaConfigurationBuilder? Builder;

    internal readonly IConfigurationProvider? ConfigurationProvider;

    private readonly List<MigrateConfigurationRelationsInfo> _relations;

    public MasaConfigurationSource(MasaConfigurationBuilder builder, List<MigrateConfigurationRelationsInfo> relations)
    {
        Builder = builder;
        _relations = relations;
    }

    public MasaConfigurationSource(IConfigurationProvider configurationProvider, List<MigrateConfigurationRelationsInfo> relations)
    {
        ConfigurationProvider = configurationProvider;
        _relations = relations;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => Builder != null ? new MasaConfigurationProvider(this, _relations) : ConfigurationProvider!;
}
