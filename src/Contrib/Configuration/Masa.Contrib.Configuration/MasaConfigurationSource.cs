// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

[ExcludeFromCodeCoverage]
public class MasaConfigurationSource : IConfigurationSource
{
    private readonly IEnumerable<IConfigurationRepository> _repositories;

    public MasaConfigurationSource(IEnumerable<IConfigurationRepository> repositories)
    {
        _repositories = repositories;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new MasaConfigurationProvider(_repositories);
}
